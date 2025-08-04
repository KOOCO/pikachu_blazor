#!/usr/bin/env -S uv run --script
# /// script
# requires-python = ">=3.8"
# dependencies = [
#     "elevenlabs",
#     "python-dotenv",
# ]
# ///

import os
import sys
import platform
import subprocess
from pathlib import Path
from dotenv import load_dotenv

def main():
    """
    ElevenLabs Turbo v2.5 TTS Script
    
    Uses ElevenLabs' Turbo v2.5 model for fast, high-quality text-to-speech.
    Accepts optional text prompt as command-line argument.
    
    Usage:
    - ./eleven_turbo_tts.py                    # Uses default text
    - ./eleven_turbo_tts.py "Your custom text" # Uses provided text
    
    Features:
    - Fast generation (optimized for real-time use)
    - High-quality voice synthesis
    - Stable production model
    - Cost-effective for high-volume usage
    """
    
    # Load environment variables
    load_dotenv()
    
    # Get API key from environment
    api_key = os.getenv('ELEVENLABS_API_KEY')
    if not api_key:
        print("❌ Error: ELEVENLABS_API_KEY not found in environment variables")
        print("Please add your ElevenLabs API key to .env file:")
        print("ELEVENLABS_API_KEY=your_api_key_here")
        sys.exit(1)
    
    try:
        from elevenlabs.client import ElevenLabs
        from elevenlabs import play
        
        # Initialize client
        elevenlabs = ElevenLabs(api_key=api_key)
        
        print("🎙️  ElevenLabs Turbo v2.5 TTS")
        print("=" * 40)
        
        # Get text from command line argument or use default
        if len(sys.argv) > 1:
            text = " ".join(sys.argv[1:])  # Join all arguments as text
        else:
            text = "The first move is what sets everything in motion."
        
        print(f"🎯 Text: {text}")
        print("🔊 Generating and playing...")
        
        try:
            # Use voice ID from environment variable or default
            voice_id = os.getenv('ELEVENLABS_VOICE_ID', 'kGjJqO6wdwRN9iJsoeIC')
            
            # Verify the voice exists in your account
            try:
                voices_response = elevenlabs.voices.get_all()
                voice_name = "Unknown Voice"
                for voice in voices_response.voices:
                    if voice.voice_id == voice_id:
                        voice_name = voice.name
                        break
                print(f"🎤 Using voice: {voice_name} ({voice_id})")
            except:
                print(f"🎤 Using voice ID: {voice_id}")
            
            # Generate and play audio directly
            audio = elevenlabs.text_to_speech.convert(
                text=text,
                voice_id=voice_id,
                model_id="eleven_turbo_v2_5",
                output_format="mp3_44100_128",
            )
            
            # Convert generator to bytes
            audio_bytes = b"".join(audio)
            
            # Save audio to temporary file first
            audio_file = "tts_output.mp3"
            with open(audio_file, 'wb') as f:
                f.write(audio_bytes)
            
            # Cross-platform audio playback
            system = platform.system().lower()
            playback_success = False
            
            if system == "darwin":  # macOS
                try:
                    subprocess.run(["/usr/bin/afplay", audio_file], check=True)
                    print("✅ Playback complete (afplay)!")
                    playback_success = True
                except Exception:
                    pass
            elif system == "windows":  # Windows
                try:
                    # Try Windows Media Player command line
                    subprocess.run(["powershell", "-c", f"(New-Object Media.SoundPlayer '{audio_file}').PlaySync()"], check=True)
                    print("✅ Playback complete (Windows Media Player)!")
                    playback_success = True
                except Exception:
                    try:
                        # Fallback to start command
                        subprocess.run(["start", audio_file], shell=True, check=True)
                        print("✅ Playback started (default audio player)!")
                        playback_success = True
                    except Exception:
                        pass
            elif system == "linux":  # Linux
                try:
                    # Try common Linux audio players
                    for player in ["paplay", "aplay", "mpg123", "ffplay"]:
                        try:
                            subprocess.run([player, audio_file], check=True)
                            print(f"✅ Playback complete ({player})!")
                            playback_success = True
                            break
                        except (subprocess.CalledProcessError, FileNotFoundError):
                            continue
                except Exception:
                    pass
            
            # Fallback to ElevenLabs native player if platform-specific methods failed
            if not playback_success:
                try:
                    play(audio_bytes)
                    print("✅ Playback complete (ElevenLabs)!")
                    playback_success = True
                except Exception as play_error:
                    print(f"⚠️ All playback methods failed: {play_error}")
                    print(f"💾 Audio saved to {audio_file} - play manually")
            
        except Exception as e:
            print(f"❌ Error: {e}")
        
        
    except ImportError:
        print("❌ Error: elevenlabs package not installed")
        print("This script uses UV to auto-install dependencies.")
        print("Make sure UV is installed: https://docs.astral.sh/uv/")
        sys.exit(1)
    except Exception as e:
        print(f"❌ Unexpected error: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()