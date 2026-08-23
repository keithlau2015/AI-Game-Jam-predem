namespace SaveLoadModule
{
    /// <summary>
    /// Which persist strategy <see cref="SaveService"/> routes to.
    /// </summary>
    public enum SaveBackendKind
    {
        /// <summary>Original template design: reflective snapshot of <see cref="SaveableModel{T}"/> maps.</summary>
        RegistrySnapshot = 0,

        /// <summary>Shipped-game style: one SaveGame document with typed DTO sections.</summary>
        DocumentDto = 1
    }
}
