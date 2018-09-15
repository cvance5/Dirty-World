using System.Collections.Generic;

namespace Items.Weapons
{
    public static class WeaponSorter
    {
        public static List<WeaponTypes> Secondaries => new List<WeaponTypes>()
        {
            WeaponTypes.DeReconstructor,
            WeaponTypes.DrillLauncher,
            WeaponTypes.SeismicBomb
        };
    }
}