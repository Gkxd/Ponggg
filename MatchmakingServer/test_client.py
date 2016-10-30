import socket
import select
import random
import time

class GameClient(object):
  def __init__(self, addr="127.0.0.1", serverport=9009):
    self.clientport = random.randrange(8000, 8999)
    self.conn = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    # Bind to localhost - set to external ip to connect from other computers
    self.conn.bind(("127.0.0.1", self.clientport))
    self.addr = addr
    self.serverport = serverport
    
    self.read_list = [self.conn]
    self.write_list = []
    
  def run(self):
    try:
      cmd = raw_input('Enter your input:')
      if cmd == "host":
        # Initialize connection to server
        name = "daniel"
        msg = cmd + ":" + name
        self.conn.sendto(msg, (self.addr, self.serverport))
        time.sleep(1)
        msg, addr = self.conn.recvfrom(4096)
        print "recvd: ", msg, addr
      elif cmd == "connect":
        name = "daniel"
        msg = cmd + ":" + name
        self.conn.sendto(msg, (self.addr, self.serverport))
        time.sleep(1)
        msg, addr = self.conn.recvfrom(4096)
        print "recvd: ", msg, addr
      elif cmd == "getUsers":
        msg = cmd
        self.conn.sendto(msg, (self.addr, self.serverport))
        time.sleep(1)
        msg, addr = self.conn.recvfrom(4096)
        print "recvd: ", msg, addr
    
    finally:
      print "done sending things"


if __name__ == "__main__":
  g = GameClient()
  g.run()
