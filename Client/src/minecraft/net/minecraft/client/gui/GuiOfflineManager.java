package net.minecraft.client.gui;

import net.minecraft.client.Minecraft;

import java.io.IOException;
import java.util.List;

public class GuiOfflineManager extends GuiScreen {

    private GuiScreen parentScreen;

    private GuiListOfflineAccounts accountList;

    private GuiButton loginButton;
    private GuiButton removeButton;
    private GuiButton addButton;
    private GuiButton backButton;

    private List<String> accounts;

    public GuiOfflineManager(GuiScreen parentScreen) {
        this.parentScreen = parentScreen;
    }

    @Override
    public void initGui() {
        accounts = OfflineAccountManager.getAccounts();

        accountList = new GuiListOfflineAccounts(
                this,
                mc,
                accounts
        );

        buttonList.clear();

        addButton = new GuiButton(
                1,
                width / 2 - 155,
                height - 52,
                100,
                20,
                "Add"
        );

        removeButton = new GuiButton(
                2,
                width / 2 - 50,
                height - 52,
                100,
                20,
                "Remove"
        );

        loginButton = new GuiButton(
                3,
                width / 2 + 55,
                height - 52,
                100,
                20,
                "Login"
        );

        backButton = new GuiButton(
                4,
                width / 2 - 100,
                height - 27,
                200,
                20,
                "Back"
        );

        buttonList.add(addButton);
        buttonList.add(removeButton);
        buttonList.add(loginButton);
        buttonList.add(backButton);

        updateButtons();
    }

    private void updateButtons() {
        boolean selected = accountList != null &&
                accountList.getSelectedIndex() >= 0 &&
                accountList.getSelectedIndex() < accounts.size();

        loginButton.enabled = selected;
        removeButton.enabled = selected;
    }

    @Override
    protected void actionPerformed(GuiButton button) throws IOException {
        if (button.id == 1) {
            mc.displayGuiScreen(new GuiOfflineAdd(this));
            return;
        }

        if (button.id == 2) {
            int index = accountList.getSelectedIndex();

            if (index >= 0 && index < accounts.size()) {
                OfflineAccountManager.removeAccount(accounts.get(index));

                initGui();
            }

            return;
        }

        if (button.id == 3) {
            int index = accountList.getSelectedIndex();

            if (index >= 0 && index < accounts.size()) {
                String username = accounts.get(index);

                if (OfflineAccountManager.login(username)) {
                    mc.displayGuiScreen(parentScreen);
                }
            }

            return;
        }

        if (button.id == 4) {
            mc.displayGuiScreen(parentScreen);
        }
    }

    @Override
    public void handleMouseInput() throws IOException {
        super.handleMouseInput();

        accountList.handleMouseInput();

        updateButtons();
    }

    @Override
    public void updateScreen() {
        super.updateScreen();

        updateButtons();
    }

    @Override
    public void drawScreen(int mouseX, int mouseY, float partialTicks) {
        drawDefaultBackground();

        drawCenteredString(
                fontRendererObj,
                "Offline Accounts",
                width / 2,
                15,
                0xFFFFFF
        );

        accountList.drawScreen(mouseX, mouseY, partialTicks);

        super.drawScreen(mouseX, mouseY, partialTicks);
    }

    public void accountSelected() {
        updateButtons();
    }

    public Minecraft getMinecraft() {
        return mc;
    }
}
