# log_service.py
import socket
import json
import yaml
import os
from typing import Dict, Any
from rate_limiter import RateLimiter
from log_formatter import LogFormatter
from log_writer import LogWriter


class LoggingService:
    def __init__(self, config_path: str = 'config.yaml'):
        self.config = self._load_config(config_path)
        self.rate_limiter = RateLimiter(self.config['logging']['rate_limit'])
        self.formatter = LogFormatter()
        self.writer = LogWriter(
            os.path.join(
                self.config['logging']['log_directory'],
                self.config['logging']['log_file']
            )
        )

    def _load_config(self, config_path: str) -> Dict[str, Any]:
        with open(config_path, 'r') as f:
            return yaml.safe_load(f)

    def start(self):
        server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server.bind((self.config['server']['host'], self.config['server']['port']))
        server.listen(5)
        print(f"Logging service started on {self.config['server']['host']}:{self.config['server']['port']}")
        print(f"Available log formats: {', '.join(self.formatter.get_available_formats())}")

        while True:
            client_socket, address = server.accept()
            self.handle_client(client_socket, address)

    def handle_client(self, client_socket: socket.socket, address: tuple):
        try:
            data = client_socket.recv(4096).decode('utf-8')
            if not data:
                return

            log_data = json.loads(data)
            log_data['client_ip'] = address[0]

            if not self.rate_limiter.check_limit(address[0]):
                error_msg = {"error": "Rate limit exceeded"}
                client_socket.send(json.dumps(error_msg).encode('utf-8'))
                return

            # Get format type from the log data
            format_type = log_data.pop('format_type', 'text')

            try:
                formatted_log = self.formatter.format_log(log_data, format_type)
                if self.writer.write_log(formatted_log):
                    response = {
                        "status": "success",
                        "message": f"Log written in {format_type} format"
                    }
                else:
                    response = {"error": "Failed to write log"}
            except ValueError as e:
                response = {"error": str(e)}

            client_socket.send(json.dumps(response).encode('utf-8'))

        except Exception as e:
            error_msg = {"error": str(e)}
            client_socket.send(json.dumps(error_msg).encode('utf-8'))
        finally:
            client_socket.close()


if __name__ == "__main__":
    service = LoggingService()
    service.start()