using System;

namespace SaveLoadModule
{
    /// <summary>
    /// UI-facing slot metadata shared by both backends.
    /// </summary>
    public sealed class SaveSlotInfo
    {
        public string SlotId { get; set; }
        public string DisplayName { get; set; }
        public long CreatedUnixMs { get; set; }
        public long UpdatedUnixMs { get; set; }
        public long FileSizeBytes { get; set; }
        public string LastLevelKey { get; set; }
        public SaveBackendKind Backend { get; set; }
        public string FileName { get; set; }

        public DateTimeOffset CreatedUtc => DateTimeOffset.FromUnixTimeMilliseconds(CreatedUnixMs);
        public DateTimeOffset UpdatedUtc =>
            UpdatedUnixMs > 0
                ? DateTimeOffset.FromUnixTimeMilliseconds(UpdatedUnixMs)
                : CreatedUtc;
    }
}
