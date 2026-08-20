from http.server import ThreadingHTTPServer, SimpleHTTPRequestHandler
import mimetypes

class UnityHandler(SimpleHTTPRequestHandler):
    def end_headers(self):
        if self.path.endswith(".br"):
            self.send_header("Content-Encoding", "br")

            if self.path.endswith(".js.br"):
                self.send_header("Content-Type", "application/javascript")
            elif self.path.endswith(".wasm.br"):
                self.send_header("Content-Type", "application/wasm")
            elif self.path.endswith(".data.br"):
                self.send_header("Content-Type", "application/octet-stream")

        super().end_headers()

server = ThreadingHTTPServer(("127.0.0.1", 8000), UnityHandler)

print("Unity WebGL server running at http://127.0.0.1:8000")
server.serve_forever()