# RegistrySnapshot

Reflective save of every `SaveableModel<T>` map into a `SaveDataModel` blob (**one file per slot**).

Entry: `RegistrySaveBackend` via `SaveService` when `SaveBackendKind.RegistrySnapshot`.

Files: `reg_{slotId}` under FileManager Save path.
