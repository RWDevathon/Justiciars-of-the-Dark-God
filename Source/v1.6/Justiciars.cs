using HarmonyLib;
using System.Reflection;
using Verse;

namespace ArtificialBeings
{
    public class Justiciars : Mod
    {
        public Justiciars(ModContentPack content) : base(content)
        {
            new Harmony("Justiciars").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}