package net.minecraft.client.gui;

import net.minecraft.client.Minecraft;

import java.util.List;

public class GuiListOfflineAccounts extends GuiSlot {

    private final GuiOfflineManager parent;
    private final List<String> accounts;

    private int selectedIndex = -1;

    public GuiListOfflineAccounts(
            GuiOfflineManager parent,
            Minecraft mc,
            List<String> accounts
    ) {
        super(
                mc,
                parent.width,
                parent.height,
                35,
                parent.height - 65,
                25
        );

        this.parent = parent;
        this.accounts = accounts;
    }

    @Override
    protected int getSize() {
        return accounts.size();
    }

    @Override
    protected void elementClicked(
            int slotIndex,
            boolean isDoubleClick,
            int mouseX,
            int mouseY
    ) {
        selectedIndex = slotIndex;
        parent.accountSelected();
    }

    @Override
    protected boolean isSelected(int slotIndex) {
        return selectedIndex == slotIndex;
    }

    @Override
    protected void drawBackground() {
    }

    @Override
    protected void drawSlot(
            int slotIndex,
            int x,
            int y,
            int height,
            int mouseX,
            int mouseY
    ) {
        String username = accounts.get(slotIndex);

        parent.drawCenteredString(
                parent.fontRendererObj,
                username,
                width / 2,
                y + 5,
                0xFFFFFF
        );
    }

    public int getSelectedIndex() {
        return selectedIndex;
    }
}
