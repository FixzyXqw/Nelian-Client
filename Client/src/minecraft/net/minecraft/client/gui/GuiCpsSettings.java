package net.minecraft.client.gui;

import net.minecraft.client.Minecraft;
import net.minecraft.client.renderer.GlStateManager;
import org.lwjgl.input.Keyboard;
import org.lwjgl.opengl.GL11;
import java.io.IOException;

public class GuiCpsSettings extends GuiScreen {

    private static final int C_BG_OVERLAY = 0x80000000;
    private static final int C_PANEL_BG = 0xFF1A1A1A;
    private static final int C_CARD_BG = 0xFF2A2A2A;
    private static final int C_CARD_HOVER = 0xFF3A3A3A;
    private static final int C_TOGGLE_ON = 0xFF8A2BE2;
    private static final int C_TOGGLE_OFF = 0xFF555555;
    private static final int C_TEXT_TITLE = 0xFFFFFFFF;
    private static final int C_TEXT_DESC = 0xFFAAAAAA;

    private static final int PANEL_W = 720;
    private static final int PANEL_H = 420;
    private static final int CARD_W = 210;
    private static final int CARD_H = 55;
    private static final int GAP = 15;

    private float scrollOffset = 0f;
    private float targetScroll = 0f;
    private float maxScroll = 0f;

    private long openTime;
    private static final int ANIM_MS = 200;

    private final GuiScreen parent;

    private int selectedColorType = -1;
    private boolean colorPickerOpen = false;
    private ColorPicker colorPicker;
    private int colorPickerX = 0;
    private int colorPickerY = 0;

    public GuiCpsSettings(GuiScreen parent) {
        this.parent = parent;
        this.openTime = System.currentTimeMillis();
    }

    private int panelX() { return (width - PANEL_W) / 2; }
    private int panelY() { return (height - PANEL_H) / 2; }

    private float computeContentHeight() {
        float h = 22f + (CARD_H + GAP) * 4;
        h += 10;
        h += 22f + (CARD_H + GAP) * 4;
        h += 15 + 20 + 40;
        h += 30;
        return h;
    }

    @Override
    public void drawScreen(int mouseX, int mouseY, float partialTicks) {
        float t = Math.min(1f, (System.currentTimeMillis() - openTime) / (float) ANIM_MS);
        float ease = 1f - (1f - t) * (1f - t) * (1f - t);
        float sc = 0.85f + 0.15f * ease;

        int px = panelX();
        int py = panelY();

        int contentHeight = PANEL_H - 75;
        float totalH = computeContentHeight();
        maxScroll = Math.max(0, totalH - contentHeight);

        scrollOffset += (targetScroll - scrollOffset) * 0.2f;
        if (Math.abs(targetScroll - scrollOffset) < 0.1f) {
            scrollOffset = targetScroll;
        }
        scrollOffset = Math.max(0, Math.min(maxScroll, scrollOffset));

        drawRect(0, 0, width, height, C_BG_OVERLAY);

        GlStateManager.pushMatrix();
        GlStateManager.translate(px + PANEL_W * 0.5f, py + PANEL_H * 0.5f, 0);
        GlStateManager.scale(sc, sc, 1.0f);
        GlStateManager.translate(-(px + PANEL_W * 0.5f), -(py + PANEL_H * 0.5f), 0);

        drawRect(px, py, px + PANEL_W, py + PANEL_H, C_PANEL_BG);
        drawRect(px, py, px + PANEL_W, py + 2, C_TOGGLE_ON);

        mc.fontRendererObj.drawString("§5§lCPS §fSettings", px + 25, py + 20, C_TEXT_TITLE);

        int backSize = 35;
        int backX = px + PANEL_W - backSize - 15;
        int backY = py + 10;
        boolean backHovered = mouseX >= backX && mouseX <= backX + backSize &&
                              mouseY >= backY && mouseY <= backY + backSize;

        drawRect(backX, backY, backX + backSize, backY + backSize, backHovered ? 0xCCEF4444 : 0x15FFFFFF);
        drawRect(backX, backY, backX + backSize, backY + 1, backHovered ? 0xFFFF5555 : 0x33FFFFFF);

        GlStateManager.pushMatrix();
        GlStateManager.scale(1.5f, 1.5f, 1.0f);
        mc.fontRendererObj.drawString("←",
                (int)((backX + backSize / 2 - 4) / 1.5f),
                (int)((backY + backSize / 2 - 4) / 1.5f),
                backHovered ? 0xFFFFFFFF : 0xFFDDDDDD);
        GlStateManager.popMatrix();

        int listStartY = py + 55;
        int startX = px + 20;

        enableScissor(px, listStartY, PANEL_W, PANEL_H - 65);
        GlStateManager.pushMatrix();
        GlStateManager.translate(0, -scrollOffset, 0);

        int curY = listStartY + 5;

        drawSectionLabel("CPS Colors", startX, curY);
        curY += 22;

        curY = drawColorPickerRow("Low CPS Color", Nelianoptions.cpsColorLow, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset), 0);
        curY += CARD_H + GAP;
        curY = drawColorPickerRow("Medium CPS Color", Nelianoptions.cpsColorMedium, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset), 1);
        curY += CARD_H + GAP;
        curY = drawColorPickerRow("High CPS Color", Nelianoptions.cpsColorHigh, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset), 2);
        curY += CARD_H + GAP;
        curY = drawColorPickerRow("Very High CPS Color", Nelianoptions.cpsColorVeryHigh, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset), 3);
        curY += CARD_H + GAP + 10;

        drawSectionLabel("Display Options", startX, curY);
        curY += 22;

        curY = drawToggleRow("Show Left Click CPS", Nelianoptions.cpsShowLeft, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset), 4);
        curY += CARD_H + GAP;
        curY = drawToggleRow("Show Right Click CPS", Nelianoptions.cpsShowRight, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset), 5);
        curY += CARD_H + GAP;
        curY = drawToggleRow("Show Total CPS", Nelianoptions.cpsShowTotal, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset), 6);
        curY += CARD_H + GAP;
        curY = drawToggleRow("Show Background", Nelianoptions.cpsShowBackground, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset), 7);
        curY += CARD_H + GAP + 15;

        drawSectionLabel("Preview", startX, curY);
        curY += 20;
        drawCpsPreview(startX + (CARD_W * 3 + GAP * 2) / 2, curY + 20, mouseX, (int)(mouseY + scrollOffset));

        GlStateManager.popMatrix();
        disableScissor();

        GlStateManager.popMatrix();

        GlStateManager.color(1.0f, 1.0f, 1.0f, 1.0f);

        if (colorPickerOpen && colorPicker != null) {
            colorPicker.setPosition(colorPickerX, colorPickerY);
            colorPicker.render(mouseX, mouseY);
        }

        GlStateManager.color(1.0f, 1.0f, 1.0f, 1.0f);
        super.drawScreen(mouseX, mouseY, partialTicks);
    }

    private void enableScissor(int x, int y, int w, int h) {
        ScaledResolution sr = new ScaledResolution(this.mc);
        int scale = sr.getScaleFactor();
        int scissorX = x * scale;
        int scissorY = mc.displayHeight - (y + h) * scale;
        GL11.glEnable(GL11.GL_SCISSOR_TEST);
        GL11.glScissor(scissorX, scissorY, w * scale, h * scale);
    }

    private void disableScissor() {
        GL11.glDisable(GL11.GL_SCISSOR_TEST);
    }

    @Override
    public void handleMouseInput() throws IOException {
        super.handleMouseInput();
        int wheel = org.lwjgl.input.Mouse.getEventDWheel();
        if (wheel != 0 && !colorPickerOpen) {
            targetScroll -= Math.signum(wheel) * 30;
            targetScroll = Math.max(0, Math.min(maxScroll, targetScroll));
        }
    }

    private void drawSectionLabel(String text, int x, int y) {
        mc.fontRendererObj.drawString(text.toUpperCase(), x, y, C_TEXT_DESC);
        drawRect(x + mc.fontRendererObj.getStringWidth(text.toUpperCase()) + 6, y + 4,
                 x + 250, y + 5, 0x22FFFFFF);
    }

    private int getCpsColor(int cps) {
        if (cps >= Nelianoptions.cpsVeryHighCPS) return Nelianoptions.cpsColorVeryHigh;
        if (cps >= Nelianoptions.cpsHighCPS) return Nelianoptions.cpsColorHigh;
        if (cps >= Nelianoptions.cpsMidCPS) return Nelianoptions.cpsColorMedium;
        return Nelianoptions.cpsColorLow;
    }

    private void drawCpsPreview(int cx, int cy, int mouseX, int mouseY) {
        int bx = cx - 80, by = cy - 20;
        drawRect(bx, by, bx + 160, by + 40, 0x33000000);
        drawRect(bx, by, bx + 160, by + 1, 0x33FFFFFF);

        String leftText = Nelianoptions.cpsShowLeft ? "L: 12" : "";
        String rightText = Nelianoptions.cpsShowRight ? "R: 8" : "";
        String totalText = Nelianoptions.cpsShowTotal ? "T: 20" : "";

        StringBuilder previewText = new StringBuilder();
        if (Nelianoptions.cpsShowLeft) previewText.append(leftText);
        if (Nelianoptions.cpsShowLeft && Nelianoptions.cpsShowRight) previewText.append("  ");
        if (Nelianoptions.cpsShowRight) previewText.append(rightText);
        if ((Nelianoptions.cpsShowLeft || Nelianoptions.cpsShowRight) && Nelianoptions.cpsShowTotal) previewText.append("  ");
        if (Nelianoptions.cpsShowTotal) previewText.append(totalText);

        String finalText = previewText.toString();
        if (finalText.isEmpty()) {
            finalText = "CPS: 0";
        }

        int textWidth = mc.fontRendererObj.getStringWidth(finalText);
        int startX = cx - textWidth / 2;

        if (Nelianoptions.cpsShowBackground) {
            drawRect(startX - 4, cy - 6, startX + textWidth + 4, cy + 10, 0x88000000);
        }

        int sampleColor = getCpsColor(20);
        mc.fontRendererObj.drawString(finalText, startX, cy - 4, sampleColor);

        mc.fontRendererObj.drawString("Preview", cx - 14, cy + 30, C_TEXT_DESC);
    }

    private int drawToggleRow(String label, boolean value, int x, int y, int w, int mouseX, int mouseY, int id) {
        boolean hRow = mouseX >= x && mouseX <= x + w && mouseY >= y && mouseY <= y + CARD_H;
        drawRect(x, y, x + w, y + CARD_H, hRow ? C_CARD_HOVER : C_CARD_BG);
        drawRect(x, y, x + w, y + 1, 0xFF3A3A3A);

        if (value) drawRect(x, y + 8, x + 3, y + CARD_H - 8, C_TOGGLE_ON);

        mc.fontRendererObj.drawString(label, x + 12, y + 8, C_TEXT_TITLE);

        int tgW = 36;
        int tgH = 20;
        int tgX = x + w - tgW - 10;
        int tgY = y + (CARD_H - tgH) / 2;

        drawRect(tgX, tgY, tgX + tgW, tgY + tgH, value ? C_TOGGLE_ON : C_TOGGLE_OFF);
        drawRect(tgX, tgY, tgX + tgW, tgY + 1, 0x66FFFFFF);
        drawRect(tgX, tgY + tgH - 1, tgX + tgW, tgY + tgH, 0x33FFFFFF);

        int thumbSize = tgH - 4;
        int thumbX = value ? tgX + tgW - thumbSize - 2 : tgX + 2;
        int thumbY = tgY + 2;
        drawRect(thumbX, thumbY, thumbX + thumbSize, thumbY + thumbSize, 0xFFFFFFFF);

        if (mouseX >= tgX && mouseX <= tgX + tgW && mouseY >= tgY && mouseY <= tgY + tgH) {
            drawRect(tgX, tgY, tgX + tgW, tgY + tgH, 0x22FFFFFF);
        }

        return y;
    }

    private int drawColorPickerRow(String label, int color, int x, int y, int w, int mouseX, int mouseY, int id) {
        boolean hRow = mouseX >= x && mouseX <= x + w && mouseY >= y && mouseY <= y + CARD_H;
        drawRect(x, y, x + w, y + CARD_H, hRow ? C_CARD_HOVER : C_CARD_BG);
        drawRect(x, y, x + w, y + 1, 0xFF3A3A3A);

        mc.fontRendererObj.drawString(label, x + 12, y + 8, C_TEXT_TITLE);

        int colorBoxX = x + w - 55;
        int colorBoxY = y + 12;
        int colorBoxSize = CARD_H - 24;

        drawRect(colorBoxX - 2, colorBoxY - 2, colorBoxX + colorBoxSize + 2, colorBoxY + colorBoxSize + 2, 0x33FFFFFF);
        drawRect(colorBoxX, colorBoxY, colorBoxX + colorBoxSize, colorBoxY + colorBoxSize, color);

        mc.fontRendererObj.drawString("▾", colorBoxX + colorBoxSize + 8, colorBoxY + 4, C_TEXT_DESC);

        return y;
    }

    @Override
    protected void mouseClicked(int mouseX, int mouseY, int mouseButton) throws IOException {
        if (mouseButton != 0) {
            super.mouseClicked(mouseX, mouseY, mouseButton);
            return;
        }

        if (colorPickerOpen && colorPicker != null) {
            boolean closed = colorPicker.mouseClicked(mouseX, mouseY, mouseButton);
            if (closed) {
                colorPickerOpen = false;
                colorPicker = null;
                selectedColorType = -1;
                Nelianoptions.save();
            }
            return;
        }

        int px = panelX();
        int py = panelY();

        int backSize = 35;
        int backX = px + PANEL_W - backSize - 15;
        int backY = py + 10;
        if (mouseX >= backX && mouseX <= backX + backSize && mouseY >= backY && mouseY <= backY + backSize) {
            Nelianoptions.save();
            Minecraft.getMinecraft().displayGuiScreen(parent);
            return;
        }

        int startX = px + 20;
        int contentW = CARD_W * 3 + GAP * 2;
        int adjustedMouseY = mouseY + (int) scrollOffset;
        int curY = py + 55 + 5;

        curY += 22;
        for (int i = 0; i <= 3; i++) {
            if (mouseX >= startX && mouseX <= startX + contentW &&
                adjustedMouseY >= curY && adjustedMouseY <= curY + CARD_H) {
                selectedColorType = i;
                int color = 0;
                switch (selectedColorType) {
                    case 0: color = Nelianoptions.cpsColorLow; break;
                    case 1: color = Nelianoptions.cpsColorMedium; break;
                    case 2: color = Nelianoptions.cpsColorHigh; break;
                    case 3: color = Nelianoptions.cpsColorVeryHigh; break;
                }
                colorPicker = new ColorPicker(colorPickerX, colorPickerY, color);
                colorPicker.setListener(new ColorPicker.ColorChangeListener() {
                    @Override
                    public void onColorChanged(int newColor) {
                        switch (selectedColorType) {
                            case 0: Nelianoptions.cpsColorLow = newColor; break;
                            case 1: Nelianoptions.cpsColorMedium = newColor; break;
                            case 2: Nelianoptions.cpsColorHigh = newColor; break;
                            case 3: Nelianoptions.cpsColorVeryHigh = newColor; break;
                        }
                    }
                });
                colorPickerX = panelX() + PANEL_W / 2 - 150;
                colorPickerY = panelY() + PANEL_H / 2 - 140;
                colorPickerOpen = true;
                return;
            }
            curY += CARD_H + GAP;
        }

        curY += 10;
        curY += 22;
        for (int i = 4; i <= 7; i++) {
            if (mouseX >= startX && mouseX <= startX + contentW &&
                adjustedMouseY >= curY && adjustedMouseY <= curY + CARD_H) {
                switch (i) {
                    case 4: Nelianoptions.cpsShowLeft = !Nelianoptions.cpsShowLeft; break;
                    case 5: Nelianoptions.cpsShowRight = !Nelianoptions.cpsShowRight; break;
                    case 6: Nelianoptions.cpsShowTotal = !Nelianoptions.cpsShowTotal; break;
                    case 7: Nelianoptions.cpsShowBackground = !Nelianoptions.cpsShowBackground; break;
                }
                Nelianoptions.save();
                return;
            }
            curY += CARD_H + GAP;
        }

        super.mouseClicked(mouseX, mouseY, mouseButton);
    }

    @Override
    protected void keyTyped(char typedChar, int keyCode) throws IOException {
        if (keyCode == Keyboard.KEY_ESCAPE) {
            if (colorPickerOpen) {
                colorPickerOpen = false;
                colorPicker = null;
                selectedColorType = -1;
                return;
            }
            Nelianoptions.save();
            Minecraft.getMinecraft().displayGuiScreen(parent);
            return;
        }
        super.keyTyped(typedChar, keyCode);
    }

    @Override
    public boolean doesGuiPauseGame() { return false; }

    @Override
    public void onGuiClosed() {
        if (!colorPickerOpen) {
            Nelianoptions.save();
        }
        super.onGuiClosed();
    }
}
