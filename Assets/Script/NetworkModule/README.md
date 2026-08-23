# NetworkModule

**Status:** Partial

## Purpose
TCP client, packet buffer, handler registry, heartbeat. Sample `ServerStatus` proto included.

## Entry points
| Type | Role |
|------|------|
| `NetworkManager` | `ConnectAsync`, `packetHandlers`, `Request` |
| `NetClient` | `TryConnect`, `Send`, `Read`, heartbeat |
| `Packet` / handlers | Protocol helpers |

## How to use
1. Set `serverIp` / `serverPort` on the component (default `127.0.0.1:45000`).
2. Leave `connectOnStart` off until you need it, or call `ConnectAsync()`.
3. Register handlers in `packetHandlers`.
4. `Request(packet)` enqueues even when the queue was empty.
5. Define Proto models + `GenericPacketHandler<T>` callbacks.

## Dependencies
Utilities `Singleton`, ProtoBuf, UniTask.
