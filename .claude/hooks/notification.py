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
import random
from pathlib import Path
from datetime import datetime

try:
    from dotenv import load_dotenv
    load_dotenv()
except ImportError:
    pass  # dotenv is optional


def get_notification_messages():
    """Return list of notification messages."""
    return [
        "Your agent needs your input",
        "Agent waiting for response",
        "Input required",
        "Agent needs guidance",
        "Your response needed"
    ]


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


def get_llm_notification_message():
    """
    Generate notification message using available LLM services.
    Priority order: OpenAI > Anthropic > fallback to random message
    
    Returns:
        str: Generated or fallback notification message
    """
    # Get current script directory and construct utils/llm path
    script_dir = Path(__file__).parent
    llm_dir = script_dir / "utils" / "llm"
    
    # Try OpenAI first (highest priority)
    if os.getenv('OPENAI_API_KEY'):
        oai_script = llm_dir / "oai.py"
        if oai_script.exists():
            try:
                result = subprocess.run([
                    "uv", "run", str(oai_script), "--notification"
                ], 
                capture_output=True,
                text=True,
                timeout=10
                )
                if result.returncode == 0 and result.stdout.strip():
                    return result.stdout.strip()
            except (subprocess.TimeoutExpired, subprocess.SubprocessError):
                pass
    
    # Try Anthropic second
    if os.getenv('ANTHROPIC_API_KEY'):
        anth_script = llm_dir / "anth.py"
        if anth_script.exists():
            try:
                result = subprocess.run([
                    "uv", "run", str(anth_script), "--notification"
                ], 
                capture_output=True,
                text=True,
                timeout=10
                )
                if result.returncode == 0 and result.stdout.strip():
                    return result.stdout.strip()
            except (subprocess.TimeoutExpired, subprocess.SubprocessError):
                pass
    
    # Get engineer name if available for personalization
    engineer_name = os.getenv('ENGINEER_NAME', '').strip()
    
    # Create notification message with 30% chance to include name
    if engineer_name and random.random() < 0.3:
        return f"{engineer_name}, your agent needs your input"
    else:
        # Fallback to random predefined message
        messages = get_notification_messages()
        return random.choice(messages)


def announce_notification():
    """Announce that the agent needs user input."""
    log_dir = os.path.join(os.getcwd(), "logs")
    os.makedirs(log_dir, exist_ok=True)
    
    try:
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Starting notification TTS announcement\n")
        
        tts_script = get_tts_script_path()
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Notification TTS script path: {tts_script}\n")
        
        if not tts_script:
            with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
                f.write(f"[{datetime.now()}] No notification TTS script found - returning\n")
            return  # No TTS scripts available
        
        # Get notification message (LLM-generated or fallback)
        notification_message = get_llm_notification_message()
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Notification message: {notification_message}\n")
        
        # Call the TTS script with the notification message
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Calling: uv run {tts_script} {notification_message}\n")
        
        # Run TTS asynchronously to avoid blocking
        result = subprocess.Popen([
            "uv", "run", tts_script, notification_message
        ], 
        stdout=subprocess.DEVNULL,
        stderr=subprocess.DEVNULL
        )
        
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Notification TTS started asynchronously with PID: {result.pid}\n")
        
    except (subprocess.TimeoutExpired, subprocess.SubprocessError, FileNotFoundError) as e:
        # Log exceptions for debugging
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Notification TTS Exception: {type(e).__name__}: {e}\n")
    except Exception as e:
        # Log any other exceptions
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Notification TTS Unexpected Exception: {type(e).__name__}: {e}\n")
    except Exception:
        # Fail silently for any other errors
        pass


def main():
    try:
        # Parse command line arguments
        parser = argparse.ArgumentParser()
        parser.add_argument('--notify', action='store_true', help='Enable TTS notifications')
        args = parser.parse_args()
        
        # Read JSON input from stdin
        input_data = json.loads(sys.stdin.read())
        
        # Ensure log directory exists
        import os
        log_dir = os.path.join(os.getcwd(), 'logs')
        os.makedirs(log_dir, exist_ok=True)
        log_file = os.path.join(log_dir, 'notification.json')
        
        # Read existing log data or initialize empty list
        if os.path.exists(log_file):
            with open(log_file, 'r') as f:
                try:
                    log_data = json.load(f)
                except (json.JSONDecodeError, ValueError):
                    log_data = []
        else:
            log_data = []
        
        # Append new data
        log_data.append(input_data)
        
        # Write back to file with formatting
        with open(log_file, 'w') as f:
            json.dump(log_data, f, indent=2)
        
        # Announce notification via TTS only if --notify flag is set
        # Skip TTS for the generic "Claude is waiting for your input" message
        if args.notify and input_data.get('message') != 'Claude is waiting for your input':
            # Add debugging log
            with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
                f.write(f"[{datetime.now()}] Calling announce_notification()\n")
            
            announce_notification()
            
            # Add completion log
            with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
                f.write(f"[{datetime.now()}] announce_notification() finished\n")
        
        sys.exit(0)
        
    except json.JSONDecodeError:
        # Handle JSON decode errors gracefully
        sys.exit(0)
    except Exception:
        # Handle any other errors gracefully
        sys.exit(0)

if __name__ == '__main__':
    main()