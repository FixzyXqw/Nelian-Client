package net.minecraft.client.gui;

import net.minecraft.client.Minecraft;
import net.minecraft.client.renderer.GlStateManager;
import org.lwjgl.input.Keyboard;
import org.lwjgl.input.Mouse;
import org.lwjgl.opengl.GL11;
import java.io.IOException;

public class GuiHitboxSettings extends GuiScreen {

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

    public GuiHitboxSettings(GuiScreen parent) {
        this.parent = parent;
        this.openTime = System.currentTimeMillis();
    }

    private int panelX() { return (width - PANEL_W) / 2; }
    private int panelY() { return (height - PANEL_H) / 2; }

    private float computeContentHeight() {
        float h = 22f + (CARD_H + GAP) * 1;
        h += 10;
        h += 22f + (CARD_H + GAP) * 1;
        h += 10;
        h += 22f + (CARD_H + GAP) * 3;
        h += 15 + 20 + 50;
        h += 30;
        return h;
    }

    @Override
    public void drawScreen(int mouseX, int mouseY, float partialTicks) {
        float t = Math.min(1f, (System.currentTimeMillis() - openTime) / (float) ANIM_MS);
        float ease = 1f - (1f - t) * (1f - t) * (1f - t);
        float sc = 0.85f + 0.15f * ease;

        int px = panelX(), py = panelY();
        int contentHeight = PANEL_H - 75;
        float totalH = computeContentHeight();
        maxScroll = Math.max(0, totalH - contentHeight);
        
        scrollOffset += (targetScroll - scrollOffset) * 0.2f;
        if (Math.abs(targetScroll - scrollOffset) < 0.1f) scrollOffset = targetScroll;
        scrollOffset = Math.max(0, Math.min(maxScroll, scrollOffset));

        drawRect(0, 0, width, height, C_BG_OVERLAY);

        GlStateManager.pushMatrix();
        GlStateManager.translate(px + PANEL_W * 0.5f, py + PANEL_H * 0.5f, 0);
        GlStateManager.scale(sc, sc, 1);
        GlStateManager.translate(-(px + PANEL_W * 0.5f), -(py + PANEL_H * 0.5f), 0);

        drawRect(px, py, px + PANEL_W, py + PANEL_H, C_PANEL_BG);
        drawRect(px, py, px + PANEL_W, py + 2, C_TOGGLE_ON);

        mc.fontRendererObj.drawString("§5§lHitbox §fSettings", px + 25, py + 20, C_TEXT_TITLE);

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

        drawSectionLabel("Hitbox Color,  §4You must enable §5Solid Color §4option to change this. ", startX, curY);
        curY += 22;

        int hitboxColor = ((int)(Nelianoptions.hitboxAlpha * 255) << 24) |
                          ((int)(Nelianoptions.hitboxRed * 255) << 16) |
                          ((int)(Nelianoptions.hitboxGreen * 255) << 8) |
                          (int)(Nelianoptions.hitboxBlue * 255);
        curY = drawColorPickerRow("Hitbox Color", hitboxColor, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset), 0);
        curY += CARD_H + GAP + 10;

        drawSectionLabel("Line Settings", startX, curY);
        curY += 22;
        
        int lineColor = ((int)(Nelianoptions.hitboxAlpha * 255) << 24) |
                        ((int)(Nelianoptions.hitboxRed * 255) << 16) |
                        ((int)(Nelianoptions.hitboxGreen * 255) << 8) |
                        (int)(Nelianoptions.hitboxBlue * 255);
        curY = drawColorPickerRow("Line Color", lineColor, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset), 1);
        curY += CARD_H + GAP + 10;

        drawSectionLabel("Render Settings", startX, curY);
        curY += 22;

        curY = drawToggleRow("Solid Color", Nelianoptions.hitboxUseCustomColor, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset));
        curY += CARD_H + GAP;
        curY = drawToggleRow("Eye Level Hitbox", Nelianoptions.hitboxShowEyeHeight, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset));
        curY += CARD_H + GAP;
        curY = drawToggleRow("Glance Vector", Nelianoptions.hitboxShowLookVector, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset));
        curY += 15;

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
        GL11.glEnable(GL11.GL_SCISSOR_TEST);
        GL11.glScissor(x * scale, this.mc.displayHeight - (y + h) * scale, w * scale, h * scale);
    }

    private void disableScissor() {
        GL11.glDisable(GL11.GL_SCISSOR_TEST);
    }

    @Override
    public void handleMouseInput() throws IOException {
        super.handleMouseInput();
        int wheel = Mouse.getEventDWheel();
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

    private int drawToggleRow(String label, boolean value, int x, int y, int w, int mouseX, int mouseY) {
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

    private int componentsToColor(float red, float green, float blue, float alpha) {
        int a = (int)(alpha * 255);
        int r = (int)(red * 255);
        int g = (int)(green * 255);
        int b = (int)(blue * 255);
        return (a << 24) | (r << 16) | (g << 8) | b;
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

        int px = panelX(), py = panelY();
        
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
        
        if (mouseX >= startX && mouseX <= startX + contentW &&
            adjustedMouseY >= curY && adjustedMouseY <= curY + CARD_H) {
            selectedColorType = 0;
            int color = componentsToColor(Nelianoptions.hitboxRed, Nelianoptions.hitboxGreen, 
                                          Nelianoptions.hitboxBlue, Nelianoptions.hitboxAlpha);
            colorPicker = new ColorPicker(colorPickerX, colorPickerY, color);
            colorPicker.setListener(new ColorPicker.ColorChangeListener() {
                @Override
                public void onColorChanged(int newColor) {
                    if (selectedColorType == 0) {
                        Nelianoptions.hitboxRed = ((newColor >> 16) & 0xFF) / 255.0f;
                        Nelianoptions.hitboxGreen = ((newColor >> 8) & 0xFF) / 255.0f;
                        Nelianoptions.hitboxBlue = (newColor & 0xFF) / 255.0f;
                        Nelianoptions.hitboxAlpha = ((newColor >> 24) & 0xFF) / 255.0f;
                    } else if (selectedColorType == 1) {
                        Nelianoptions.hitboxRed = ((newColor >> 16) & 0xFF) / 255.0f;
                        Nelianoptions.hitboxGreen = ((newColor >> 8) & 0xFF) / 255.0f;
                        Nelianoptions.hitboxBlue = (newColor & 0xFF) / 255.0f;
                        Nelianoptions.hitboxAlpha = ((newColor >> 24) & 0xFF) / 255.0f;
                    }
                }
            });
            colorPickerX = panelX() + PANEL_W / 2 - 150;
            colorPickerY = panelY() + PANEL_H / 2 - 140;
            colorPickerOpen = true;
            return;
        }
        curY += CARD_H + GAP + 10;

        curY += 22;
        
        if (mouseX >= startX && mouseX <= startX + contentW &&
            adjustedMouseY >= curY && adjustedMouseY <= curY + CARD_H) {
            selectedColorType = 1;
            int color = componentsToColor(Nelianoptions.hitboxRed, Nelianoptions.hitboxGreen, 
                                          Nelianoptions.hitboxBlue, Nelianoptions.hitboxAlpha);
            colorPicker = new ColorPicker(colorPickerX, colorPickerY, color);
            colorPicker.setListener(new ColorPicker.ColorChangeListener() {
                @Override
                public void onColorChanged(int newColor) {
                    if (selectedColorType == 0) {
                        Nelianoptions.hitboxRed = ((newColor >> 16) & 0xFF) / 255.0f;
                        Nelianoptions.hitboxGreen = ((newColor >> 8) & 0xFF) / 255.0f;
                        Nelianoptions.hitboxBlue = (newColor & 0xFF) / 255.0f;
                        Nelianoptions.hitboxAlpha = ((newColor >> 24) & 0xFF) / 255.0f;
                    } else if (selectedColorType == 1) {
                        Nelianoptions.hitboxRed = ((newColor >> 16) & 0xFF) / 255.0f;
                        Nelianoptions.hitboxGreen = ((newColor >> 8) & 0xFF) / 255.0f;
                        Nelianoptions.hitboxBlue = (newColor & 0xFF) / 255.0f;
                        Nelianoptions.hitboxAlpha = ((newColor >> 24) & 0xFF) / 255.0f;
                    }
                }
            });
            colorPickerX = panelX() + PANEL_W / 2 - 150;
            colorPickerY = panelY() + PANEL_H / 2 - 140;
            colorPickerOpen = true;
            return;
        }
        curY += CARD_H + GAP + 10;

        curY += 22;
        
        if (mouseX >= startX && mouseX <= startX + contentW) {
            if (adjustedMouseY >= curY && adjustedMouseY <= curY + CARD_H) {
                Nelianoptions.hitboxUseCustomColor = !Nelianoptions.hitboxUseCustomColor;
                Nelianoptions.save();
                return;
            }
            curY += CARD_H + GAP;
            if (adjustedMouseY >= curY && adjustedMouseY <= curY + CARD_H) {
                Nelianoptions.hitboxShowEyeHeight = !Nelianoptions.hitboxShowEyeHeight;
                Nelianoptions.save();
                return;
            }
            curY += CARD_H + GAP;
            if (adjustedMouseY >= curY && adjustedMouseY <= curY + CARD_H) {
                Nelianoptions.hitboxShowLookVector = !Nelianoptions.hitboxShowLookVector;
                Nelianoptions.save();
                return;
            }
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
    public void onGuiClosed() {
        if (!colorPickerOpen) {
            Nelianoptions.save();
        }
        super.onGuiClosed();
    }

    @Override
    public boolean doesGuiPauseGame() { return false; }
}
