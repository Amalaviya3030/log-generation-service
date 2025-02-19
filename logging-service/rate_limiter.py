# rate_limiter.py
import time
from typing import Dict, List


class RateLimiter:
    def __init__(self, limit_per_second: int):
        self.limit = limit_per_second
        self.clients: Dict[str, List[float]] = {}

    def check_limit(self, client_ip: str) -> bool:
        current_time = time.time()

        if client_ip not in self.clients:
            self.clients[client_ip] = []

        # Remove timestamps older than 1 second
        self.clients[client_ip] = [ts for ts in self.clients[client_ip]
                                   if current_time - ts <= 1.0]

        # Check if under rate limit
        if len(self.clients[client_ip]) < self.limit:
            self.clients[client_ip].append(current_time)
            return True

        return False

    def reset_client(self, client_ip: str):
        if client_ip in self.clients:
            del self.clients[client_ip]