#!/usr/bin/env -S uv run --script
# /// script
# requires-python = ">=3.11"
# dependencies = [
#     "python-dotenv",
# ]
# ///

import json
import sys
import re
import os
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


def announce_security_alert(message):
    """Announce security alert using TTS."""
    log_dir = os.path.join(os.getcwd(), "logs")
    os.makedirs(log_dir, exist_ok=True)
    
    try:
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Starting security alert TTS\n")
        
        tts_script = get_tts_script_path()
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Security alert TTS script path: {tts_script}\n")
        
        if not tts_script:
            with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
                f.write(f"[{datetime.now()}] No security alert TTS script found - returning\n")
            return  # No TTS scripts available
        
        # Use security alert message
        alert_message = f"Security Alert: {message}"
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Security alert message: {alert_message}\n")
        
        # Call the TTS script with the alert message
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Calling: uv run {tts_script} {alert_message}\n")
        
        # Run TTS asynchronously to avoid blocking
        result = subprocess.Popen([
            "uv", "run", tts_script, alert_message
        ], 
        stdout=subprocess.DEVNULL,
        stderr=subprocess.DEVNULL
        )
        
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Security alert TTS started asynchronously with PID: {result.pid}\n")
        
    except (subprocess.TimeoutExpired, subprocess.SubprocessError, FileNotFoundError) as e:
        # Log exceptions for debugging
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Security alert TTS Exception: {type(e).__name__}: {e}\n")
    except Exception as e:
        # Log any other exceptions
        with open(os.path.join(log_dir, 'tts_debug.log'), 'a') as f:
            f.write(f"[{datetime.now()}] Security alert TTS Unexpected Exception: {type(e).__name__}: {e}\n")
    except Exception:
        # Fail silently for any other errors
        pass


def is_dangerous_rm_command(command):
    """
    Comprehensive detection of dangerous rm commands.
    Matches various forms of rm -rf and similar destructive patterns.
    """
    # Normalize command by removing extra spaces and converting to lowercase
    normalized = ' '.join(command.lower().split())
    
    # Pattern 1: Standard rm -rf variations
    patterns = [
        r'\brm\s+.*-[a-z]*r[a-z]*f',  # rm -rf, rm -fr, rm -Rf, etc.
        r'\brm\s+.*-[a-z]*f[a-z]*r',  # rm -fr variations
        r'\brm\s+--recursive\s+--force',  # rm --recursive --force
        r'\brm\s+--force\s+--recursive',  # rm --force --recursive
        r'\brm\s+-r\s+.*-f',  # rm -r ... -f
        r'\brm\s+-f\s+.*-r',  # rm -f ... -r
    ]
    
    # Check for dangerous patterns
    for pattern in patterns:
        if re.search(pattern, normalized):
            return True
    
    # Pattern 2: Check for rm with recursive flag targeting dangerous paths
    dangerous_paths = [
        r'/',           # Root directory
        r'/\*',         # Root with wildcard
        r'~',           # Home directory
        r'~/',          # Home directory path
        r'\$HOME',      # Home environment variable
        r'\.\.',        # Parent directory references
        r'\*',          # Wildcards in general rm -rf context
        r'\.',          # Current directory
        r'\.\s*$',      # Current directory at end of command
    ]
    
    if re.search(r'\brm\s+.*-[a-z]*r', normalized):  # If rm has recursive flag
        for path in dangerous_paths:
            if re.search(path, normalized):
                return True
    
    return False

def is_env_file_access(tool_name, tool_input):
    """
    Check if any tool is trying to access .env files containing sensitive data.
    """
    if tool_name in ['Read', 'Edit', 'MultiEdit', 'Write', 'Bash']:
        # Check file paths for file-based tools
        if tool_name in ['Read', 'Edit', 'MultiEdit', 'Write']:
            file_path = tool_input.get('file_path', '')
            if '.env' in file_path and not file_path.endswith('.env.sample'):
                return True
        
        # Check bash commands for .env file access
        elif tool_name == 'Bash':
            command = tool_input.get('command', '')
            # Pattern to detect .env file access (but allow .env.sample)
            env_patterns = [
                r'\b\.env\b(?!\.sample)',  # .env but not .env.sample
                r'cat\s+.*\.env\b(?!\.sample)',  # cat .env
                r'echo\s+.*>\s*\.env\b(?!\.sample)',  # echo > .env
                r'touch\s+.*\.env\b(?!\.sample)',  # touch .env
                r'cp\s+.*\.env\b(?!\.sample)',  # cp .env
                r'mv\s+.*\.env\b(?!\.sample)',  # mv .env
            ]
            
            for pattern in env_patterns:
                if re.search(pattern, command):
                    return True
    
    return False

def main():
    try:
        # Read JSON input from stdin
        input_data = json.load(sys.stdin)
        
        tool_name = input_data.get('tool_name', '')
        tool_input = input_data.get('tool_input', {})
        
        # Check for .env file access (blocks access to sensitive environment files)
        if is_env_file_access(tool_name, tool_input):
            error_msg = "Access to .env files containing sensitive data is prohibited"
            print(f"BLOCKED: {error_msg}", file=sys.stderr)
            print("Use .env.sample for template files instead", file=sys.stderr)
            
            # Announce security alert via TTS
            announce_security_alert("Sensitive file access blocked")
            
            sys.exit(2)  # Exit code 2 blocks tool call and shows error to Claude
        
        # Check for dangerous rm -rf commands
        if tool_name == 'Bash':
            command = tool_input.get('command', '')
            
            # Block rm -rf commands with comprehensive pattern matching
            if is_dangerous_rm_command(command):
                error_msg = "Dangerous rm command detected and prevented"
                print(f"BLOCKED: {error_msg}", file=sys.stderr)
                
                # Announce security alert via TTS
                announce_security_alert("Dangerous command blocked")
                
                sys.exit(2)  # Exit code 2 blocks tool call and shows error to Claude
        
        # Ensure log directory exists
        log_dir = Path.cwd() / 'logs'
        log_dir.mkdir(parents=True, exist_ok=True)
        log_path = log_dir / 'pre_tool_use.json'
        
        # Read existing log data or initialize empty list
        if log_path.exists():
            with open(log_path, 'r') as f:
                try:
                    log_data = json.load(f)
                except (json.JSONDecodeError, ValueError):
                    log_data = []
        else:
            log_data = []
        
        # Append new data
        log_data.append(input_data)
        
        # Write back to file with formatting
        with open(log_path, 'w') as f:
            json.dump(log_data, f, indent=2)
        
        sys.exit(0)
        
    except json.JSONDecodeError:
        # Gracefully handle JSON decode errors
        sys.exit(0)
    except Exception:
        # Handle any other errors gracefully
        sys.exit(0)

if __name__ == '__main__':
    main()