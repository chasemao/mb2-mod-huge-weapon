using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem.ViewModelCollection.Craft.WeaponDesign;
using System.Collections.ObjectModel;
using TaleWorlds.Library;
using TaleWorlds.Engine;
using Newtonsoft.Json;


namespace HugeWeapon
{
    public class Main : MBSubModuleBase
    {
        private Harmony harmonyKit;
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            this.harmonyKit = new Harmony("HugeWeapon.harmony");
            this.harmonyKit.PatchAll();
            InformationManager.DisplayMessage(new InformationMessage("HugeWeapon loaded"));
        }
        public override void OnGameLoaded(Game game, object initializerObject)
        {
            InformationManager.DisplayMessage(new InformationMessage("HugeWeapon loaded"));
        }
    }
    [HarmonyPatch(typeof(Crafting), "GenerateItem")]
    public class PatchGenerateItem
    {
        public static void Prefix(ref WeaponDesign weaponDesign, ref Crafting.OverrideData overridenData)
        {
            if (weaponDesign == null || weaponDesign.UsedPieces == null || weaponDesign.UsedPieces.Length == 0)
            {
                return;
            }
            if (weaponDesign.WeaponName == null || weaponDesign.WeaponName.Length == 0)
            {
                return;
            }
            if (!weaponDesign.WeaponName.Contains("Crafted") && !weaponDesign.WeaponName.Contains("打造"))
            {
                return;
            }
            foreach (WeaponDesignElement weaponDesignElement in weaponDesign.UsedPieces)
            {
                weaponDesignElement.CraftingPiece.AdditionalWeaponFlags |= WeaponFlags.BonusAgainstShield | WeaponFlags.CanKnockDown | WeaponFlags.CanCrushThrough;
                if (weaponDesignElement.CraftingPiece.PieceType == CraftingPiece.PieceTypes.Blade)
                {
                    weaponDesignElement.ScalePercentage = 1000;
                }
                if (weaponDesignElement.CraftingPiece.PieceType == CraftingPiece.PieceTypes.Guard)
                {
                    weaponDesignElement.ScalePercentage = 200;
                }
                if (weaponDesignElement.CraftingPiece.PieceType == CraftingPiece.PieceTypes.Handle)
                {
                    weaponDesignElement.ScalePercentage = 200;
                }
                if (weaponDesignElement.CraftingPiece.PieceType == CraftingPiece.PieceTypes.Pommel)
                {
                    weaponDesignElement.ScalePercentage = 200;
                }
            }
            WeaponDesignElement[] usedPieces = new WeaponDesignElement[weaponDesign.UsedPieces.Length];
            for (int index = 0; index < weaponDesign.UsedPieces.Length; ++index)
            {
                usedPieces[index] = weaponDesign.UsedPieces[index].GetCopy();
            }
            weaponDesign = new WeaponDesign(weaponDesign.Template, weaponDesign.WeaponName, usedPieces);
            float oriWeight = MathF.Round(((IEnumerable<WeaponDesignElement>)weaponDesign.UsedPieces).Sum<WeaponDesignElement>((Func<WeaponDesignElement, float>)(selectedUsablePiece => selectedUsablePiece.ScaledWeight)), 2);
            if (overridenData == null)
            {
                overridenData = new Crafting.OverrideData();
            }
            overridenData.WeightOverriden = 10f - oriWeight;
            overridenData.SwingSpeedOverriden = 100;
            overridenData.SwingDamageOverriden = 2000;
            overridenData.ThrustSpeedOverriden = 100;
            overridenData.ThrustDamageOverriden = 2000;
            overridenData.Handling = 500;
        }
    }
}
