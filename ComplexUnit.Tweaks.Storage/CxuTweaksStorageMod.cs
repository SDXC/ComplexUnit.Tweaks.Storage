using Mafi;
using Mafi.Base;
using Mafi.Collections;
using Mafi.Core.Buildings.Shipyard;
using Mafi.Core.Buildings.Storages;
using Mafi.Core.Game;
using Mafi.Core.Mods;
using Mafi.Core.Prototypes;
using Mafi.Serialization;


namespace ComplexUnit.Tweaks.Storage
{
    public sealed class CxuStorageCapacityMod : IMod
    {
        private string Name => nameof(CxuStorageCapacityMod);
        public int Version => 1;

        public ModManifest Manifest { get; }
        public bool IsUiOnly => false;
        public Option<IConfig> ModConfig => Option<IConfig>.None;
        public ModJsonConfig JsonConfig { get; }



        public bool EnableCustomStorageCapacity => this.JsonConfig.GetBool("enable_custom_storage_capacity");
        public int StorageCapacityT1 => this.JsonConfig.GetInt("storage_capacity_t1");
        public int StorageCapacityT2 => this.JsonConfig.GetInt("storage_capacity_t2");
        public int StorageCapacityT3 => this.JsonConfig.GetInt("storage_capacity_t3");
        public int StorageCapacityT4 => this.JsonConfig.GetInt("storage_capacity_t4");
        public int StorageCapacityNuclearWaste => this.JsonConfig.GetInt("storage_capacity_nuclear_waste");
        public int StorageCapacityShipyard => this.JsonConfig.GetInt("storage_capacity_shipyard");
        public bool AllowChilledWaterInStorage => this.JsonConfig.GetBool("allow_chilled_water_in_fluid_storage");
        public bool AllowExhaustInStorage => this.JsonConfig.GetBool("allow_exhaust_in_fluid_storage");
        public bool AllowFBRFuelsInStorage => this.JsonConfig.GetBool("allow_fbr_fuels_in_fluid_storage");
        public bool AllowMicrochipStagesInStorage => this.JsonConfig.GetBool("allow_microchip_stages_in_solid_storage");
        public bool AllowSteamInStorage => this.JsonConfig.GetBool("allow_steam_in_fluid_storage");



        public CxuStorageCapacityMod(ModManifest manifest)
        {
            this.Manifest = manifest;
            this.JsonConfig = new ModJsonConfig(this);

            Log.Info($"{this.Name}: Mod loaded!");
        }



        public void EarlyInit(DependencyResolver resolver)
        {
            return;
        }

        public void MigrateJsonConfig(VersionSlim savedVersion, Dict<string, object> savedValues)
        {
            return;
        }

        public void RegisterPrototypes(ProtoRegistrator registrator)
        {
            if (this.EnableCustomStorageCapacity)
            {
                Log.Info($"{this.Name}: Registering new capacities for storage buildings...");

                Quantity newcapacity;

                newcapacity = this.StorageCapacityT1.Quantity();
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageFluid).Value, "Capacity", newcapacity);
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageLoose).Value, "Capacity", newcapacity);
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageUnit).Value, "Capacity", newcapacity);

                newcapacity = this.StorageCapacityT2.Quantity();
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageFluidT2).Value, "Capacity", newcapacity);
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageLooseT2).Value, "Capacity", newcapacity);
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageUnitT2).Value, "Capacity", newcapacity);

                newcapacity = this.StorageCapacityT3.Quantity();
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageFluidT3).Value, "Capacity", newcapacity);
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageLooseT3).Value, "Capacity", newcapacity);
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageUnitT3).Value, "Capacity", newcapacity);

                newcapacity = this.StorageCapacityT4.Quantity();
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageFluidT4).Value, "Capacity", newcapacity);
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageLooseT4).Value, "Capacity", newcapacity);
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<StorageProto>(Ids.Buildings.StorageUnitT4).Value, "Capacity", newcapacity);

                newcapacity = this.StorageCapacityNuclearWaste.Quantity();
                ReflectionUtils.SetField<StorageBaseProto>(registrator.PrototypesDb.Get<NuclearWasteStorageProto>(Ids.Buildings.NuclearWasteStorage).Value, "Capacity", newcapacity);

                foreach (StorageBaseProto storage in registrator.PrototypesDb.All<StorageBaseProto>())
                {
                    StorageBaseProto proto = registrator.PrototypesDb.Get<StorageBaseProto>(storage.Id).Value;

                    ReflectionUtils.SetField<StorageBaseProto>(proto, nameof(proto.TransferLimit), 100_000.Quantity());
                    ReflectionUtils.SetField<StorageBaseProto>(proto, nameof(proto.TransferLimitDuration), 1.Ticks());

                    StorageProto storproto = proto as StorageProto;
                    if (storproto != null)
                        ReflectionUtils.SetField<StorageProto>(storproto, "<" + nameof(storproto.ThroughputPerTick) + ">k__BackingField", 100_000.Quantity().AsPartial / 1.Ticks().Ticks);
                }

                newcapacity = this.StorageCapacityShipyard.Quantity();
                ReflectionUtils.SetField<ShipyardProto>(registrator.PrototypesDb.Get<ShipyardProto>(Ids.Buildings.Shipyard).Value, "CargoCapacity", newcapacity);
                ReflectionUtils.SetField<ShipyardProto>(registrator.PrototypesDb.Get<ShipyardProto>(Ids.Buildings.Shipyard2).Value, "CargoCapacity", newcapacity);

                Log.Info($"{this.Name}: Finished registering new capacities.");
            }

            if (this.AllowChilledWaterInStorage)
            {
                Log.Info($"{this.Name}: Registering chilled water to storage list...");

                Utility.AddToStorableProducts(registrator.PrototypesDb, new[] { Ids.Products.ChilledWater });

                Log.Info($"{this.Name}: Finished registering chilled water.");
            }

            if (this.AllowExhaustInStorage)
            {
                Log.Info($"{this.Name}: Registering exhaust to storage list...");

                Utility.AddToStorableProducts(registrator.PrototypesDb, new[] { Ids.Products.Exhaust });

                Log.Info($"{this.Name}: Finished registering exhaust.");
            }

            if (this.AllowFBRFuelsInStorage)
            {
                Log.Info($"{this.Name}: Registering FBR fuel types to storage list...");

                Utility.AddToStorableProducts(registrator.PrototypesDb, new[] { Ids.Products.BlanketFuel, Ids.Products.BlanketFuelEnriched, Ids.Products.CoreFuel, Ids.Products.CoreFuelDirty });

                Log.Info($"{this.Name}: Finished registering FBR fuel types.");
            }

            if (this.AllowMicrochipStagesInStorage)
            {
                Log.Info($"{this.Name}: Registering microchip stages to storage list...");

                Utility.AddToStorableProducts(registrator.PrototypesDb, new[] { Ids.Products.MicrochipsStage1A, Ids.Products.MicrochipsStage2A, Ids.Products.MicrochipsStage3A, Ids.Products.MicrochipsStage4A, Ids.Products.MicrochipsStage1B, Ids.Products.MicrochipsStage2B, Ids.Products.MicrochipsStage3B, Ids.Products.MicrochipsStage4B, Ids.Products.MicrochipsStage1C, Ids.Products.MicrochipsStage2C, Ids.Products.MicrochipsStage3C });

                Log.Info($"{this.Name}: Finished registering microchip stages.");
            }

            if (this.AllowSteamInStorage)
            {
                Log.Info($"{this.Name}: Registering steam to storage list...");

                Utility.AddToStorableProducts(registrator.PrototypesDb, new[] { Ids.Products.SteamSp, Ids.Products.SteamHi, Ids.Products.SteamLo, Ids.Products.SteamDepleted });

                Log.Info($"{this.Name}: Finished registering steam.");
            }
        }

        public void RegisterDependencies(DependencyResolverBuilder depBuilder, ProtosDb protosDb, bool gameWasLoaded)
        {
            return;
        }

        public void Initialize(DependencyResolver resolver, bool gameWasLoaded)
        {
            return;
        }

        public void Dispose()
        {
            return;
        }
    }
}