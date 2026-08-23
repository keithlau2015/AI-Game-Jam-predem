using AttributeModule;
using Model;
using System;
using System.Collections.Generic;
using System.Numerics;
namespace FormulaModule
{
    public class FormulaController
    {
        public enum FormulaType
        {
            StdAtkDmg = 1,
            GunfireDmg = 2,
            LaserDmg = 3,
            MissleDmg = 4,
            MineDmg = 5
        }

        private static readonly string HP_ATTRIBUTE_NAME = "hp";
        private static readonly string MP_ATTRIBUTE_NAME = "mp";
        private static readonly string ATK_ATTRIBUTE_NAME = "atk";
        private static readonly string DEF_ATTRIBUTE_NAME = "def";
        private static readonly string CR_ATTRIBUTEI_NAME = "cri";
        private static readonly string CRI_DMG_ATTRIBUTE_NAME = "criDmg";
        private static readonly string DODGE_ATTRIBUTE_NAME = "dodge";
        private static readonly string SPD_ATTRIBUTE_NAME = "spd";
        private static readonly string SHIELD_ATTRIBUTE_NAME = "shield";
        private static readonly string SHIELD_REGEN_SPD_ATTRIBUTE_NAME = "sheildRegenSpd";

        public static BigInteger GetDmg(IAttributeHolder attacker, IAttributeHolder defender, int formula)
        {
            UnityEngine.Debug.Log($"Get Dmg {formula}");
            switch (formula)
            {
                case (int)FormulaType.StdAtkDmg:
                    return StdAtkDmg(attacker, defender);
                case (int)FormulaType.GunfireDmg:
                    return GunfireDmg(attacker, defender);
                case (int)FormulaType.LaserDmg:
                    return LaserDmg(attacker, defender);
                case (int)FormulaType.MissleDmg:
                    return MissleDmg(attacker, defender);
                case (int)FormulaType.MineDmg:
                    return MineDmg(attacker, defender);
            }

            return 0;
        }

        private static AttributeData ConvertVariable2AttributeData(string variable, Dictionary<int, AttributeData> attributes)
        {
            AttributeData attributeData = null;
            if (variable.Equals(HP_ATTRIBUTE_NAME))
            {
                attributes.TryGetValue((int)AttributeModel.AttributeType.HP, out attributeData);
            }
            else if (variable.Equals(MP_ATTRIBUTE_NAME))
            {
                attributes.TryGetValue((int)AttributeModel.AttributeType.MP, out attributeData);
            }
            else if (variable.Equals(ATK_ATTRIBUTE_NAME))
            {
                attributes.TryGetValue((int)AttributeModel.AttributeType.ATK, out attributeData);
            }
            else if (variable.Equals(DEF_ATTRIBUTE_NAME))
            {
                attributes.TryGetValue((int)AttributeModel.AttributeType.DEF, out attributeData);
            }
            else if (variable.Equals(CR_ATTRIBUTEI_NAME))
            {
                attributes.TryGetValue((int)AttributeModel.AttributeType.CRI, out attributeData);
            }
            else if (variable.Equals(CRI_DMG_ATTRIBUTE_NAME))
            {
                attributes.TryGetValue((int)AttributeModel.AttributeType.CRI_DMG, out attributeData);
            }
            else if (variable.Equals(DODGE_ATTRIBUTE_NAME))
            {
                attributes.TryGetValue((int)AttributeModel.AttributeType.DODGE, out attributeData);
            }
            else if (variable.Equals(SPD_ATTRIBUTE_NAME))
            {
                attributes.TryGetValue((int)AttributeModel.AttributeType.SPD, out attributeData);
            }
            else if (variable.Equals(SHIELD_ATTRIBUTE_NAME))
            {
                attributes.TryGetValue((int)AttributeModel.AttributeType.SHIELD, out attributeData);
            }
            else if (variable.Equals(SHIELD_REGEN_SPD_ATTRIBUTE_NAME))
            {
                attributes.TryGetValue((int)AttributeModel.AttributeType.SHIELD_REGEN_SPD, out attributeData);
            }

            return attributeData;
        }

        private static BigInteger StdAtkDmg(IAttributeHolder attacker, IAttributeHolder defender)
        {
            AttributeData atk = null;
            AttributeData def = null;
            AttributeData dodge = null;
            AttributeData cri = null;
            AttributeData criDmg = null;
            AttributeData shield = null;
            AttributeData hp = null;
            AttributeData shieldEfficiency = null;
            AttributeData hit = null;

            if (!attacker.attributes.TryGetValue((int)AttributeModel.AttributeType.ATK, out atk))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.ATK.ToString()));
            }

            if (!defender.attributes.TryGetValue((int)AttributeModel.AttributeType.DEF, out def))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.DEF.ToString()));
            }

            if (!defender.attributes.TryGetValue((int)AttributeModel.AttributeType.HP, out hp))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.HP.ToString()));
            }

            if (!defender.attributes.TryGetValue((int)AttributeModel.AttributeType.DODGE, out dodge))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.DODGE.ToString()));
            }

            if (!defender.attributes.TryGetValue((int)AttributeModel.AttributeType.SHIELD, out shield))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.SHIELD.ToString()));
            }

            if (!attacker.attributes.TryGetValue((int)AttributeModel.AttributeType.CRI, out cri))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.CRI.ToString()));
            }

            if (!attacker.attributes.TryGetValue((int)AttributeModel.AttributeType.CRI_DMG, out criDmg))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.CRI_DMG.ToString()));
            }

            if (!attacker.attributes.TryGetValue((int)AttributeModel.AttributeType.HIT, out hit))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.HIT.ToString()));
            }

            if (!defender.attributes.TryGetValue((int)AttributeModel.AttributeType.SHIELD_EFFICIENCY, out shieldEfficiency))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.SHIELD_EFFICIENCY.ToString()));
            }

            BigInteger dmg = 0;

            float criRNG = UnityEngine.Random.Range(1f, 101f);
            BigInteger criChk = new BigInteger(criRNG) + hit.value - dodge.value;

            if (criChk > dodge.value && criChk < shieldEfficiency.value)
            {
                float correction = UnityEngine.Random.Range(90f, 101f);
                dmg = atk.value - def.value * (new BigInteger(correction) * new BigInteger(0.01f));
            }
            else if (criChk > shieldEfficiency.value)
            {
                float correction = UnityEngine.Random.Range(95, 106);
                dmg = atk.value * (new BigInteger(correction) * new BigInteger(0.01f));
            }
            else if (criChk > (shieldEfficiency.value + dodge.value + 30))
            {
                float correction = UnityEngine.Random.Range(95, 106);
                dmg = atk.value * 2 * (new BigInteger(correction) * new BigInteger(0.01f));
            }

            if (dmg <= 0)
            {
                dmg = 1;
            }

            UnityEngine.Debug.Log($"CriRNG: {criRNG}, CriChk: {criChk}, Dodge: {dodge.value}, ShieldEff: {shieldEfficiency.value}, Dmg: {dmg}");

            return dmg;
        }

        private static BigInteger GunfireDmg(IAttributeHolder attacker, IAttributeHolder defender)
        {
            AttributeData atk = null;
            AttributeData def = null;

            if (!attacker.attributes.TryGetValue((int)AttributeModel.AttributeType.ATK, out atk))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.ATK.ToString()));
            }

            if (!defender.attributes.TryGetValue((int)AttributeModel.AttributeType.DEF, out def))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.DEF.ToString()));
            }

            float correction = UnityEngine.Random.Range(90, 101) * 0.01f;
            BigInteger dmg = (atk.value - def.value) * new BigInteger(correction);
            return dmg;
        }

        private static BigInteger LaserDmg(IAttributeHolder attacker, IAttributeHolder defender)
        {
            AttributeData atk = null;
            AttributeData def = null;

            if (!attacker.attributes.TryGetValue((int)AttributeModel.AttributeType.ATK, out atk))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.ATK.ToString()));
            }

            if (!defender.attributes.TryGetValue((int)AttributeModel.AttributeType.DEF, out def))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.DEF.ToString()));
            }

            float correction = UnityEngine.Random.Range(90, 101) * 0.01f;
            BigInteger dmg = (atk.value - def.value) * new BigInteger(correction);
            return dmg;
        }

        private static BigInteger MissleDmg(IAttributeHolder attacker, IAttributeHolder defender)
        {
            AttributeData atk = null;
            AttributeData def = null;

            if (!attacker.attributes.TryGetValue((int)AttributeModel.AttributeType.ATK, out atk))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.ATK.ToString()));
            }

            if (!defender.attributes.TryGetValue((int)AttributeModel.AttributeType.DEF, out def))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.DEF.ToString()));
            }

            float correction = UnityEngine.Random.Range(50, 101) * 0.01f;
            BigInteger dmg = atk.value * new BigInteger(correction);
            return dmg;
        }

        private static BigInteger MineDmg(IAttributeHolder attacker, IAttributeHolder defender)
        {
            AttributeData atk = null;
            AttributeData def = null;

            if (!attacker.attributes.TryGetValue((int)AttributeModel.AttributeType.ATK, out atk))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.ATK.ToString()));
            }

            if (!defender.attributes.TryGetValue((int)AttributeModel.AttributeType.DEF, out def))
            {
                throw new FormatException(string.Format(ErrorCode.MissngFormulaAttribute, AttributeModel.AttributeType.DEF.ToString()));
            }

            float correction = UnityEngine.Random.Range(50, 101) * 0.01f;
            BigInteger dmg = atk.value * new BigInteger(correction);
            return dmg;
        }
    }
}