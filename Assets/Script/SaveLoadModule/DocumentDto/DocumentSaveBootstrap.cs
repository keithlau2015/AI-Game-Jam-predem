using System.Collections.Generic;
using SaveLoadModule.DocumentDto.Participants;

namespace SaveLoadModule.DocumentDto
{
    /// <summary>
    /// Wires template gameplay systems into the DocumentDto backend.
    /// Games can call <see cref="DocumentSaveBackend.RegisterParticipant"/> for extra sections.
    /// </summary>
    public static class DocumentSaveBootstrap
    {
        public static void RegisterDefaultParticipants(List<ISaveParticipant> participants)
        {
            participants.Add(new MetaSaveParticipant());
            participants.Add(new ItemSaveParticipant());
            participants.Add(new EquipmentSaveParticipant());
            participants.Add(new CombatUnitSaveParticipant());
            participants.Add(new CombatUnitSlotSaveParticipant());
            participants.Add(new SkinSaveParticipant());
            participants.Add(new AchievementSaveParticipant());
            participants.Add(new EvtRecordSaveParticipant());
        }
    }
}
