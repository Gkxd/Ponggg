import socket
import select
import sys

# Messages:
#  Client->Server
#   One or two characters. First character is the command:
#     c: connect
#     u: update position
#     d: disconnect
#   Second character only applies to position and specifies direction (udlr)
#
#  Server->Client
#   '|' delimited pairs of positions to draw the players (there is no
#     distinction between the players - not even the client knows where its
#     player is.

class GameServer(object):
  def __init__(self, port=9009, max_num_players=100):
    self.listener = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    # Bind to localhost - set to external ip to connect from other computers
    self.listener.bind(("127.0.0.1", port))
    self.read_list = [self.listener]
    self.write_list = []
    
    self.players = dict()
    
  def run(self):
    print "Waiting..."
    try:
      while True:
        readable, writable, exceptional = (
          select.select(self.read_list, self.write_list, [])
        )
        for f in readable:
          if f is self.listener:
            msg, addr = f.recvfrom(4096)
            print msg, addr
            try:
              params = msg.split(':')
              action = params[0]
              if(action == "host"):
                name = params[1]
                print name
                if(name not in self.players):
                  self.players[name] = addr
                  self.listener.sendto("confirmed hosting", addr)
                else:
                  self.listener.sendto("name already taken, failed to register as host", addr)
              if(action == "connect"):
                opponentName = params[1]
                print opponentName
                if(opponentName not in self.players):
                  self.listener.sendto("this name is not hosting a match", addr)
                else:
                  self.listener.sendto("connectning to host at " + str(addr), addr)
              if(action == "getUsers"):
                  self.listener.sendto("the current users are " + str(self.players.keys()), addr)
            except ValueError:
              self.listener.sendto("failed to establish connection", addr)
              print "user rejected"
              

            """"
            if len(msg) >= 1:
              cmd = msg[0]
              if cmd == "c":  # New Connection
                self.players[addr] = (0,0)
              elif cmd == "u":  # Movement Update
                if len(msg) >= 2 and addr in self.players:
                  # Second char of message is direction (udlr)
                  self.do_movement(msg[1], addr)
              elif cmd == "d":  # Player Quitting
                if addr in self.players:
                  del self.players[addr]
              else:
                print "Unexpected: {0}".format(msg)
            """
        """
        for player in self.players:
          send = []
          for pos in self.players:
            send.append("{0},{1}".format(*self.players[pos]))
          self.listener.sendto('|'.join(send), player)
        """
    except KeyboardInterrupt as e:
      pass

if __name__ == "__main__":
  g = GameServer()
  g.run()
