# log_formatter.py
from datetime import datetime
import json
import csv
from io import StringIO
from typing import Dict, Any


class LogFormatter:
    def __init__(self):
        self.format_handlers = {
            'text': self._format_text,
            'json': self._format_json,
            'csv': self._format_csv,
            'key-value': self._format_key_value
        }
        # Define the standard field order
        self.field_order = ['timestamp', 'level', 'client_id', 'client_ip', 'message']

    def format_log(self, log_data: Dict[str, Any], format_type: str) -> str:
        if format_type not in self.format_handlers:
            raise ValueError(f"Unsupported format type: {format_type}")

        # Add timestamp if not present
        if 'timestamp' not in log_data:
            log_data['timestamp'] = datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3]

        return self.format_handlers[format_type](log_data)

    def _format_text(self, log_data: Dict[str, Any]) -> str:
        return f"[{log_data['timestamp']}] [{log_data['level']}] [{log_data['client_id']}@{log_data['client_ip']}] {log_data['message']}"

    def _format_json(self, log_data: Dict[str, Any]) -> str:
        # Create ordered dictionary to maintain field order
        ordered_data = {field: log_data.get(field, '') for field in self.field_order}
        # Add any additional fields not in standard order
        for key, value in log_data.items():
            if key not in ordered_data:
                ordered_data[key] = value
        return json.dumps(ordered_data)

    def _format_csv(self, log_data: Dict[str, Any]) -> str:
        output = StringIO()
        writer = csv.writer(output)
        # Use field_order to maintain consistent ordering
        writer.writerow([log_data.get(field, '') for field in self.field_order])
        return output.getvalue().strip()

    def _format_key_value(self, log_data: Dict[str, Any]) -> str:
        # Create ordered dictionary to maintain field order
        ordered_data = {field: log_data.get(field, '') for field in self.field_order}
        # Add any additional fields not in standard order
        for key, value in log_data.items():
            if key not in ordered_data:
                ordered_data[key] = value
        return ' '.join([f"{k}={v}" for k, v in ordered_data.items()])

    def get_available_formats(self) -> list:
        return list(self.format_handlers.keys())