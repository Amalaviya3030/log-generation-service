# server.py
import socket


def start_server():
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

    host = '0.0.0.0'  # Listen on all available interfaces
    port = 5000

    server_socket.bind((host, port))
    server_socket.listen(1)
    print(f"Server listening on port {port}")

    while True:
        client_socket, address = server_socket.accept()
        print(f"Connection from {address}")
        client_socket.close()


if __name__ == "__main__":
    start_server()