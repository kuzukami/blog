package com.nomurabbit.mymod.core.init;

import com.nomurabbit.mymod.MyMod;
import net.minecraft.world.food.FoodProperties;
import net.minecraft.world.item.*;
import net.minecraftforge.fmllegacy.RegistryObject;
import net.minecraftforge.registries.DeferredRegister;
import net.minecraftforge.registries.ForgeRegistries;
import net.minecraft.world.item.CreativeModeTab;

public final class ItemInit {

    public static final DeferredRegister<Item> ITEMS = DeferredRegister.create(ForgeRegistries.ITEMS,
            MyMod.MOD_ID);

    public static final RegistryObject<Item> EXAMPLE_ITEM = ITEMS.register("example_item",
            () -> new Item(new Item.Properties().tab(MyMod.TUTORIAL_TAB).fireResistant()));

    public static final RegistryObject<Item> EXAMPLE_FOOD = ITEMS.register("example_food",
            () -> new Item(new Item.Properties().tab(MyMod.TUTORIAL_TAB).food(
                    new FoodProperties.Builder().nutrition(4).saturationMod(0.3F).build()).fireResistant()));

    public static final RegistryObject<Item> CANDY_CANE = ITEMS.register("candy_cane",
            () -> new SwordItem(Tiers.NETHERITE, 195, -2.4F,
                    (new Item.Properties().tab(MyMod.TUTORIAL_TAB).fireResistant())));

    public static final RegistryObject<Item> CANDY_CANE_AXE = ITEMS.register("candy_cane_axe",
            () -> new AxeItem(Tiers.NETHERITE, 195, -2.4F,
                    (new Item.Properties().tab(MyMod.TUTORIAL_TAB).fireResistant())));

    private ItemInit() {
    }
}
