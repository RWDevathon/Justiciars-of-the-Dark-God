using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    // This Thing will create darkness around it, and periodically update it. It will remove darkness upon despawning, and force other instances to update at that time.
    [StaticConstructorOnStartup]
    public class BlackVeil : ThingWithComps
    {
        private float radius = 3.49f;

        private static readonly Material DarknessMat = MaterialPool.MatFrom("Things/Gas/GasCloudThickA", ShaderDatabase.TransparentPostLight);
        private readonly MaterialPropertyBlock[] DarknessPropertyBlocks = new MaterialPropertyBlock[4];
        private readonly int[] angles = new int[4];
        private readonly int[] rotationRates = new int[4];

        public int ticksLeft = 1000;

        public float Radius
        {
            get
            {
                return radius;
            }
            set
            {
                if (!Spawned)
                {
                    radius = value;
                    return;
                }
                // Expanding the grid is easy. Just update the grid after the size change.
                if (value >= radius)
                {
                    radius = value;
                    UpdateGrid(true, false);
                }
                // Shrinking the grid requires removing cells. Notify neighboring veils to update their grids and update.
                else
                {
                    MapComponent_DarknessGrid darkGrid = Map.GetComponent<MapComponent_DarknessGrid>();
                    List<BlackVeil> neighboringVeils = new List<BlackVeil>();
                    for (int i = darkGrid.blackVeils.Count - 1; i >= 0; i--)
                    {
                        if (darkGrid.blackVeils[i] != this && darkGrid.blackVeils[i].Spawned && radius + darkGrid.blackVeils[i].radius >= Position.DistanceTo(darkGrid.blackVeils[i].Position))
                        {
                            neighboringVeils.Add(darkGrid.blackVeils[i]);
                        }
                    }
                    UpdateGrid(false, false);
                    radius = value;
                    UpdateGrid(true, false);
                    for (int i = neighboringVeils.Count; i >= 0; i--)
                    {
                        neighboringVeils[i].UpdateGrid(true, false);
                    }
                }
            }
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            // The Darkness Grid area, which shows only if hovered over.
            if (Position == UI.MouseCell())
            {
                GenDraw.DrawRadiusRing(Position, radius, Color.black);
            }
            // The cloud effects
            drawLoc.y = AltitudeLayer.MoteOverheadLow.AltitudeFor();
            for (int i = DarknessPropertyBlocks.Length - 1; i >= 0; i--)
            {
                Vector3 s = new Vector3(2 * radius, 1f, 2 * radius);
                Matrix4x4 matrix = default;
                matrix.SetTRS(drawLoc, Quaternion.AngleAxis(angles[i], Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, DarknessMat, 0, null, 0, DarknessPropertyBlocks[i]);
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            Map.GetComponent<MapComponent_DarknessGrid>().blackVeils.AddUnique(this);
            UpdateGrid(true, false);
            // Set up graphics
            for (int i = 0; i < DarknessPropertyBlocks.Length; i++)
            {
                DarknessPropertyBlocks[i] = new MaterialPropertyBlock();
                DarknessPropertyBlocks[i].SetColor(ShaderPropertyIDs.Color, Color.black.WithAlpha(Rand.Range(0.9f, 1.15f)));
                angles[i] = Rand.Range(0, 360);
                rotationRates[i] = 0;
                while (rotationRates[i] == 0)
                {
                    rotationRates[i] = Rand.Range(-3, 3);
                }
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            UpdateGrid(false, true);
            Map.GetComponent<MapComponent_DarknessGrid>().blackVeils.Remove(this);
            base.DeSpawn(mode);
        }

        protected override void Tick()
        {
            base.Tick();
            for (int i = DarknessPropertyBlocks.Length - 1; i >= 0; i--)
            {
                angles[i] += rotationRates[i];
            }
            if (--ticksLeft <= 0)
            {
                Destroy();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref radius, "ABF_blackVeilRadius");
            Scribe_Values.Look(ref ticksLeft, "ABF_ticksLeft");
        }

        public void UpdateGrid(bool isDark, bool checkOverlapping)
        {
            MapComponent_DarknessGrid darkGrid = Map.GetComponent<MapComponent_DarknessGrid>();
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(Position, radius, true))
            {
                darkGrid.SetDark(cell, isDark);
            }

            if (checkOverlapping)
            {
                for (int i = darkGrid.blackVeils.Count - 1; i >= 0; i--)
                {
                    if (darkGrid.blackVeils[i] != this && darkGrid.blackVeils[i].Spawned && radius + darkGrid.blackVeils[i].radius >= Position.DistanceTo(darkGrid.blackVeils[i].Position))
                    {
                        // Forcing other veils to update can only force them to "repaint" the grid and not disassemble themselves or force other veils to update.
                        darkGrid.blackVeils[i].UpdateGrid(true, false);
                    }
                }
            }
        }
    }
}
