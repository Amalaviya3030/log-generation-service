# log_writer.py
import os
from typing import Dict, Any

class LogWriter:
    def __init__(self, log_file: str):
        self.log_file = log_file
        self._ensure_log_directory()

    def _ensure_log_directory(self):
        os.makedirs(os.path.dirname(self.log_file), exist_ok=True)

    def write_log(self, formatted_log: str):
        try:
            with open(self.log_file, 'a') as f:
                f.write(formatted_log + '\n')
            return True
        except Exception as e:
            print(f"Error writing to log file: {e}")
            return False

    def set_log_file(self, log_file: str):
        self.log_file = log_file
        self._ensure_log_directory()