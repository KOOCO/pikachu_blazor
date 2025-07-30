#!/usr/bin/env -S uv run --script
# /// script
# requires-python = ">=3.11"
# dependencies = [
#     "python-dotenv",
# ]
# ///

import argparse
import json
import os
import sys
import subprocess
from pathlib import Path
from datetime import datetime

try:
    from dotenv import load_dotenv
    load_dotenv()
except ImportError:
    pass  # dotenv is optional


def get_tts_script_path():
    """
    Determine which TTS script to use based on available API keys.
    Priority order: ElevenLabs > OpenAI > pyttsx3
    """
    # Get current script directory and construct utils/tts path
    script_dir = Path(__file__).parent
    tts_dir = script_dir / "utils" / "tts"
    
    # Check for ElevenLabs API key (highest priority)
    if os.getenv('ELEVENLABS_API_KEY'):
        elevenlabs_script = tts_dir / "elevenlabs_tts.py"
        if elevenlabs_script.exists():
            return str(elevenlabs_script)
    
    # Check for OpenAI API key (second priority)
    if os.getenv('OPENAI_API_KEY'):
        openai_script = tts_dir / "openai_tts.py"
        if openai_script.exists():
            return str(openai_script)
    
    # Fall back to pyttsx3 (no API key required)
    pyttsx3_script = tts_dir / "pyttsx3_tts.py"
    if pyttsx3_script.exists():
        return str(pyttsx3_script)
    
    return None


def announce_prompt_blocked(reason):
    """Announce blocked prompt using TTS."""
    log_dir = os.path.join(os.getcwd(), "logs")
    os.makedirs(log_dir, exist_ok=True)
    
    try:
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Starting prompt blocked TTS\n")
        
        tts_script = get_tts_script_path()
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Prompt blocked TTS script path: {tts_script}\n")
        
        if not tts_script:
            with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
                f.write(f"[{datetime.now()}] No prompt blocked TTS script found - returning\n")
            return  # No TTS scripts available
        
        # Use prompt blocked message
        blocked_message = f"Prompt blocked: {reason}"
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Prompt blocked message: {blocked_message}\n")
        
        # Call the TTS script with the blocked message
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Calling: uv run {tts_script} {blocked_message}\n")
        
        # Run TTS asynchronously to avoid blocking
        result = subprocess.Popen([
            "uv", "run", tts_script, blocked_message
        ], 
        stdout=subprocess.DEVNULL,
        stderr=subprocess.DEVNULL
        )
        
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Prompt blocked TTS started asynchronously with PID: {result.pid}\n")
        
    except (subprocess.TimeoutExpired, subprocess.SubprocessError, FileNotFoundError) as e:
        # Log exceptions for debugging
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Prompt blocked TTS Exception: {type(e).__name__}: {e}\n")
    except Exception as e:
        # Log any other exceptions
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Prompt blocked TTS Unexpected Exception: {type(e).__name__}: {e}\n")
    except Exception:
        # Fail silently for any other errors
        pass


def log_user_prompt(session_id, input_data):
    """Log user prompt to logs directory."""
    # Ensure logs directory exists
    log_dir = Path("logs")
    log_dir.mkdir(parents=True, exist_ok=True)
    log_file = log_dir / 'user_prompt_submit.json'
    
    # Read existing log data or initialize empty list
    if log_file.exists():
        with open(log_file, 'r') as f:
            try:
                log_data = json.load(f)
            except (json.JSONDecodeError, ValueError):
                log_data = []
    else:
        log_data = []
    
    # Append the entire input data
    log_data.append(input_data)
    
    # Write back to file with formatting
    with open(log_file, 'w') as f:
        json.dump(log_data, f, indent=2)


def validate_prompt(prompt):
    """
    Validate the user prompt for security or policy violations.
    Returns tuple (is_valid, reason).
    """
    # Example validation rules (customize as needed)
    blocked_patterns = [
        # Add any patterns you want to block
        # Example: ('rm -rf /', 'Dangerous command detected'),
    ]
    
    prompt_lower = prompt.lower()
    
    for pattern, reason in blocked_patterns:
        if pattern.lower() in prompt_lower:
            return False, reason
    
    return True, None


def main():
    try:
        # Parse command line arguments
        parser = argparse.ArgumentParser()
        parser.add_argument('--validate', action='store_true', 
                          help='Enable prompt validation')
        parser.add_argument('--log-only', action='store_true',
                          help='Only log prompts, no validation or blocking')
        args = parser.parse_args()
        
        # Read JSON input from stdin
        input_data = json.loads(sys.stdin.read())
        
        # Extract session_id and prompt
        session_id = input_data.get('session_id', 'unknown')
        prompt = input_data.get('prompt', '')
        
        # Log the user prompt
        log_user_prompt(session_id, input_data)
        
        # Validate prompt if requested and not in log-only mode
        if args.validate and not args.log_only:
            is_valid, reason = validate_prompt(prompt)
            if not is_valid:
                # Exit code 2 blocks the prompt with error message
                print(f"Prompt blocked: {reason}", file=sys.stderr)
                
                # Announce prompt blocked via TTS
                announce_prompt_blocked(reason)
                
                sys.exit(2)
        
        # Add context information (optional)
        # You can print additional context that will be added to the prompt
        # Example: print(f"Current time: {datetime.now()}")
        
        # Success - prompt will be processed
        sys.exit(0)
        
    except json.JSONDecodeError:
        # Handle JSON decode errors gracefully
        sys.exit(0)
    except Exception:
        # Handle any other errors gracefully
        sys.exit(0)


if __name__ == '__main__':
    main()