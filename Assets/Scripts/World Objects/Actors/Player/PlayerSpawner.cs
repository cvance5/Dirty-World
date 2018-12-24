using Characters;
using UI;
using UI.Overlays;
using UnityEngine;
using WorldObjects.Actors.Player.Guns;

namespace WorldObjects.Actors.Player
{
    public class PlayerSpawner : Singleton<PlayerSpawner>
    {
#pragma warning disable IDE0044 // Add readonly modifier, Unity serialization requires it not be readonly
        [SerializeField]
        private GameObject _playerBase = null;

        [SerializeField]
        private GameObject _seismicBombLauncherActor = null;
        [SerializeField]
        private GameObject _drillDroneLauncherActor = null;
        [SerializeField]
        private GameObject _deReconstructorActor = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public static GameObject SpawnPlayer(Equipment equipment)
        {
            var player = Instantiate(Instance._playerBase);
            Gun secondaryWeapon;

            switch (equipment.EquippedSecondaryWeapon)
            {
                case null:
                    secondaryWeapon = null;
                    break;

                case Items.Weapons.WeaponTypes.DrillLauncher:
                    var drillLauncher = Instantiate(Instance._drillDroneLauncherActor);
                    secondaryWeapon = drillLauncher.GetComponent<DrillLauncher>();
                    break;

                case Items.Weapons.WeaponTypes.DeReconstructor:
                    var dereconstructor = Instantiate(Instance._deReconstructorActor);
                    secondaryWeapon = dereconstructor.GetComponent<DeReconstructor>();
                    break;

                case Items.Weapons.WeaponTypes.SeismicBomb:
                    var seismicBombLauncher = Instantiate(Instance._seismicBombLauncherActor);
                    secondaryWeapon = seismicBombLauncher.GetComponent<SeismicBombLauncher>();
                    break;

                default: throw new System.ArgumentException($"Invalid Secondary Weapon of type {equipment.EquippedSecondaryWeapon}.");
            }

            var playerWeapons = player.GetComponent<PlayerWeapons>();
            playerWeapons.EquipSecondary(secondaryWeapon);

            var playerData = player.GetComponent<PlayerData>();
            var healthOverlay = UIManager.Get<HealthOverlay>();
            healthOverlay.AssignPlayer(playerData);

            return player;
        }
    }
}