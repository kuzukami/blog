package com.nomurabbit.mymod.core.init;

import com.nomurabbit.mymod.MyMod;
import net.minecraft.world.item.Item;
import net.minecraft.world.item.SwordItem;
import net.minecraft.world.item.Tiers;
import net.minecraft.world.item.ItemStack;
import net.minecraftforge.fmllegacy.RegistryObject;
import net.minecraftforge.registries.DeferredRegister;
import net.minecraftforge.registries.ForgeRegistries;
import net.minecraft.world.item.CreativeModeTab;

public final class ItemInit {

    public static final CreativeModeTab TUTORIAL_TAB = new CreativeModeTab(MyMod.MOD_ID) {
        @Override
        public ItemStack makeIcon() {
            return EXAMPLE_ITEM.get().getDefaultInstance();
        }
    };

    public static final DeferredRegister<Item> ITEMS = DeferredRegister.create(ForgeRegistries.ITEMS,
            MyMod.MOD_ID);

    public static final RegistryObject<Item> EXAMPLE_ITEM = ITEMS.register("example_item",
            () -> new Item(new Item.Properties().tab(TUTORIAL_TAB).fireResistant()));

    public static final RegistryObject<Item> EXAMPLE_SWORD = ITEMS.register("example_sword",
            () -> new SwordItem(Tiers.NETHERITE, 5, -2.4F,
                    (new Item.Properties().tab(TUTORIAL_TAB).fireResistant())));

    private ItemInit() {
    }
}
