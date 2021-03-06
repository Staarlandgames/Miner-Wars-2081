﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Models;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabConfigurationBankNode : MyPrefabConfiguration
    {
        public MyPrefabConfigurationBankNode(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, MyModelsEnum? collisionModelEnum = null)
            : this(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, PrefabTypesFlagEnum.Default, collisionModelEnum)
        {
        }

        public MyPrefabConfigurationBankNode(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, PrefabTypesFlagEnum prefabTypeFlag, MyModelsEnum? collisionModelEnum = null)
            : base(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, prefabTypeFlag, collisionModelEnum: collisionModelEnum, requiresEnergy: true, displayHud: true)
        {
        }
    }
}
