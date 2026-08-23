# DocumentDto

Shipped-game SaveGame document: versioned root + JSON sections owned by `ISaveParticipant`.

Entry: `DocumentSaveBackend` via `SaveService` when `SaveBackendKind.DocumentDto`.

Files: `dto_{slotId}` under FileManager Save path.

Extend: implement `ISaveParticipant`, register in `DocumentSaveBootstrap` or at runtime.
