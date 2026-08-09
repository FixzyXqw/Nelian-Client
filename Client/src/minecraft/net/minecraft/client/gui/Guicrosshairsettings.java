package net.minecraft.client.gui;

import net.minecraft.client.Minecraft;
import net.minecraft.client.renderer.GlStateManager;
import org.lwjgl.input.Keyboard;
import org.lwjgl.input.Mouse;
import org.lwjgl.opengl.GL11;
import java.io.IOException;

public class Guicrosshairsettings extends GuiScreen {

    // ========== RENKLER ==========
    private static final int C_BG_OVERLAY = 0x80000000;
    private static final int C_PANEL_BG = 0xFF1A1A1A;
    private static final int C_CARD_BG = 0xFF2A2A2A;
    private static final int C_CARD_HOVER = 0xFF3A3A3A;
    private static final int C_TOGGLE_ON = 0xFF8A2BE2;
    private static final int C_TOGGLE_OFF = 0xFF555555;
    private static final int C_TEXT_TITLE = 0xFFFFFFFF;
    private static final int C_TEXT_DESC = 0xFFAAAAAA;

    // ========== LAYOUT ==========
    private static final int PANEL_W = 720;
    private static final int PANEL_H = 420;
    private static final int CARD_W = 210;
    private static final int CARD_H = 55;
    private static final int GAP = 15;

    // ========== SCROLL ==========
    private float scrollOffset = 0f;
    private float targetScroll = 0f;
    private float maxScroll = 0f;

    // ========== ANİMASYON ==========
    private long openTime;
    private static final int ANIM_MS = 200;

    // ========== GERİ DÖNÜŞ ==========
    private final GuiScreen parent;

    // ========== RENK SEÇİCİ ==========
    private boolean colorPickerOpen = false;
    private ColorPicker colorPicker;
    private int colorPickerX = 0;
    private int colorPickerY = 0;

    // ========== SLIDER SÜRÜKLEME ==========
    private String draggingSlider = null;
    private int dragStartX = 0;
    private int dragStartValue = 0;

    public Guicrosshairsettings(GuiScreen parent) {
        this.parent = parent;
        this.openTime = System.currentTimeMillis();
    }

    private int panelX() { return (width - PANEL_W) / 2; }
    private int panelY() { return (height - PANEL_H) / 2; }

    private float computeContentHeight() {
        float h = 22f + (CARD_H + GAP) * 4;
        h += 10;
        h += 22f + (CARD_H + GAP) * 3;
        h += 6;
        h += 22f + (CARD_H + GAP) * 2;
        h += 15 + 20 + 40;
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

        mc.fontRendererObj.drawString("§5§lCrosshair §fSettings", px + 25, py + 20, C_TEXT_TITLE);

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

        // Color bölümü - COLORPICKER EKLENDİ
        drawSectionLabel("Color", startX, curY);
        curY += 22;

        int prevSz = 30;
        int prevX = startX + CARD_W * 3 + GAP * 2 - prevSz - 5;
        int previewColor = Nelianoptions.getCrosshairARGB();
        drawRect(prevX - 2, curY - 2, prevX + prevSz + 2, curY + prevSz + 2, 0x33FFFFFF);
        drawRect(prevX, curY, prevX + prevSz, curY + prevSz, previewColor);

        int contentW = CARD_W * 3 + GAP * 2 - prevSz - 15;

        // Color Picker Row
        curY = drawColorPickerRow("Crosshair Color", previewColor, startX, curY, contentW, mouseX, (int)(mouseY + scrollOffset));
        curY += 10;

        // Style bölümü
        drawSectionLabel("Style", startX, curY);
        curY += 22;
        curY = drawIntSlider("Gap", Nelianoptions.crosshairGap, 0, 20, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset));
        curY = drawIntSlider("Length", Nelianoptions.crosshairLength, 1, 30, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset));
        curY = drawIntSlider("Thickness", Nelianoptions.crosshairThickness, 1, 10, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset));

        curY += 10;

        // Options bölümü
        drawSectionLabel("Options", startX, curY);
        curY += 22;
        curY = drawToggleRow("MidPoint", Nelianoptions.crosshairDot, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset));
        curY += CARD_H + GAP;
        curY = drawToggleRow("Rainbow", Nelianoptions.crosshairRainbow, startX, curY, CARD_W * 3 + GAP * 2, mouseX, (int)(mouseY + scrollOffset));
        curY += 15;

        // Preview
        drawSectionLabel("Preview", startX, curY + 45);
        curY += 65;
        drawCrosshairPreview(startX + (CARD_W * 3 + GAP * 2) / 2, curY + 20, mouseX, (int)(mouseY + scrollOffset));

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

    private int drawColorPickerRow(String label, int color, int x, int y, int w, int mouseX, int mouseY) {
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

        return y + CARD_H + GAP;
    }

    private int drawIntSlider(String label, int value, int min, int max,
                               int x, int y, int w, int mouseX, int mouseY) {
        int labelW = mc.fontRendererObj.getStringWidth(label);
        int sliderX = x + labelW + 10;
        int sliderW = w - labelW - 50;

        boolean hRow = mouseX >= x && mouseX <= x + w && mouseY >= y && mouseY <= y + CARD_H;
        drawRect(x, y, x + w, y + CARD_H, hRow ? C_CARD_HOVER : C_CARD_BG);
        drawRect(x, y, x + w, y + 1, 0xFF3A3A3A);

        mc.fontRendererObj.drawString(label, x + 10, y + 8, C_TEXT_TITLE);

        int trackY = y + CARD_H / 2 - 7;
        drawRect(sliderX, trackY, sliderX + sliderW, trackY + 14, 0xFF353540);

        float ratio = (float)(value - min) / (max - min);
        int fillW = (int)(sliderW * ratio);
        if (fillW > 0) {
            drawRect(sliderX, trackY, sliderX + fillW, trackY + 14, C_TOGGLE_ON);
        }

        int knobX = sliderX + fillW;
        drawRect(knobX - 6, trackY - 2, knobX + 6, trackY + 16, 0xFFFFFFFF);
        drawRect(knobX - 4, trackY, knobX + 4, trackY + 14, C_TOGGLE_ON);

        int valX = sliderX + sliderW + 8;
        mc.fontRendererObj.drawString(String.valueOf(value), valX, y + CARD_H / 2 - 4, C_TEXT_DESC);

        return y + CARD_H + GAP;
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

    private void drawCrosshairPreview(int cx, int cy, int mouseX, int mouseY) {
        int color = Nelianoptions.getCrosshairARGB();

        int len = Nelianoptions.crosshairLength;
        int gap = Nelianoptions.crosshairGap;
        int thick = Math.max(1, Nelianoptions.crosshairThickness);
        boolean dot = Nelianoptions.crosshairDot;

        int half = thick / 2;
        int extra = (gap == 0 ? 1 : 0);

        int bx = cx - 32;
        int by = cy - 20;

        drawRect(bx, by, bx + 64, by + 40, 0x33000000);
        drawRect(bx, by, bx + 64, by + 1, 0x33FFFFFF);

        GlStateManager.pushMatrix();
        GlStateManager.enableBlend();
        GlStateManager.tryBlendFuncSeparate(770, 771, 1, 0);
        GlStateManager.disableTexture2D();

        // SOL
        drawRect(cx - gap - len, cy - half, cx - gap, cy - half + thick, color);
        // SAĞ
        drawRect(cx + gap, cy - half, cx + gap + len + extra, cy - half + thick, color);
        // ÜST
        drawRect(cx - half, cy - gap - len, cx - half + thick, cy - gap, color);
        // ALT
        drawRect(cx - half, cy + gap, cx - half + thick, cy + gap + len + extra, color);

        if (dot) {
            drawRect(cx - half, cy - half, cx - half + thick, cy - half + thick, color);
        }

        GlStateManager.enableTexture2D();
        GlStateManager.disableBlend();
        GlStateManager.popMatrix();

        mc.fontRendererObj.drawString("Preview", cx - mc.fontRendererObj.getStringWidth("Preview") / 2, cy + 30, C_TEXT_DESC);
    }

    // ========== MOUSE HANDLERS ==========
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

        int mouseYAdj = (int)(mouseY + scrollOffset);
        int startX = px + 20;
        int contentW = CARD_W * 3 + GAP * 2;
        int curY = py + 55 + 5 + 22;

        // Color Picker Row
        if (mouseX >= startX && mouseX <= startX + contentW &&
            mouseYAdj >= curY && mouseYAdj <= curY + CARD_H) {
            int color = Nelianoptions.getCrosshairARGB();
            colorPicker = new ColorPicker(colorPickerX, colorPickerY, color);
            colorPicker.setListener(new ColorPicker.ColorChangeListener() {
                @Override
                public void onColorChanged(int newColor) {
                    Nelianoptions.crosshairColor = newColor;
                }
            });
            colorPickerX = panelX() + PANEL_W / 2 - 150;
            colorPickerY = panelY() + PANEL_H / 2 - 140;
            colorPickerOpen = true;
            return;
        }
        curY += CARD_H + GAP + 10;

        // Style slider'ları
        // Gap slider
        if (mouseX >= startX && mouseX <= startX + contentW &&
            mouseYAdj >= curY && mouseYAdj <= curY + CARD_H) {
            draggingSlider = "gap";
            dragStartX = mouseX;
            dragStartValue = Nelianoptions.crosshairGap;
            handleSliderDrag(mouseX, "gap");
            return;
        }
        curY += CARD_H + GAP;
        
        // Length slider
        if (mouseX >= startX && mouseX <= startX + contentW &&
            mouseYAdj >= curY && mouseYAdj <= curY + CARD_H) {
            draggingSlider = "length";
            dragStartX = mouseX;
            dragStartValue = Nelianoptions.crosshairLength;
            handleSliderDrag(mouseX, "length");
            return;
        }
        curY += CARD_H + GAP;
        
        // Thickness slider
        if (mouseX >= startX && mouseX <= startX + contentW &&
            mouseYAdj >= curY && mouseYAdj <= curY + CARD_H) {
            draggingSlider = "thickness";
            dragStartX = mouseX;
            dragStartValue = Nelianoptions.crosshairThickness;
            handleSliderDrag(mouseX, "thickness");
            return;
        }
        curY += CARD_H + GAP + 10 + 22;

        // Toggle tıklamaları
        int toggleY = curY;
        if (mouseX >= startX && mouseX <= startX + contentW) {
            if (mouseYAdj >= toggleY && mouseYAdj <= toggleY + CARD_H) {
                Nelianoptions.crosshairDot = !Nelianoptions.crosshairDot;
                Nelianoptions.save();
                return;
            }
            toggleY += CARD_H + GAP;
            if (mouseYAdj >= toggleY && mouseYAdj <= toggleY + CARD_H) {
                Nelianoptions.crosshairRainbow = !Nelianoptions.crosshairRainbow;
                Nelianoptions.save();
                return;
            }
        }

        super.mouseClicked(mouseX, mouseY, mouseButton);
    }

    @Override
    protected void mouseClickMove(int mouseX, int mouseY, int clickedMouseButton, long timeSinceLastClick) {
        if (clickedMouseButton == 0 && draggingSlider != null) {
            handleSliderDrag(mouseX, draggingSlider);
        }
        super.mouseClickMove(mouseX, mouseY, clickedMouseButton, timeSinceLastClick);
    }

    @Override
    protected void mouseReleased(int mouseX, int mouseY, int state) {
        if (state == 0 && draggingSlider != null) {
            draggingSlider = null;
            Nelianoptions.save();
        }
        super.mouseReleased(mouseX, mouseY, state);
    }

    private void handleSliderDrag(int mouseX, String slider) {
        int px = panelX();
        int startX = px + 20;
        int contentW = CARD_W * 3 + GAP * 2;
        
        int labelW = mc.fontRendererObj.getStringWidth(getSliderLabel(slider));
        int sliderX = startX + labelW + 10;
        int sliderW = contentW - labelW - 50;
        
        int min = 0, max = 0;
        String target = "";
        
        switch (slider) {
            case "gap":
                min = 0;
                max = 20;
                target = "gap";
                break;
            case "length":
                min = 1;
                max = 30;
                target = "length";
                break;
            case "thickness":
                min = 1;
                max = 10;
                target = "thickness";
                break;
        }
        
        // Mouse pozisyonunu slider aralığına çevir
        float relativeX = Math.max(0, Math.min(1, (float)(mouseX - sliderX) / sliderW));
        int newValue = min + (int)((max - min) * relativeX);
        
        // Değeri sınırla
        newValue = Math.max(min, Math.min(max, newValue));
        
        // Değeri uygula
        switch (target) {
            case "gap":
                Nelianoptions.crosshairGap = newValue;
                break;
            case "length":
                Nelianoptions.crosshairLength = newValue;
                break;
            case "thickness":
                Nelianoptions.crosshairThickness = Math.max(1, newValue);
                break;
        }
    }
    
    private String getSliderLabel(String slider) {
        switch (slider) {
            case "gap": return "Gap";
            case "length": return "Length";
            case "thickness": return "Thickness";
            default: return "";
        }
    }

    @Override
    protected void keyTyped(char typedChar, int keyCode) throws IOException {
        if (keyCode == Keyboard.KEY_ESCAPE) {
            if (colorPickerOpen) {
                colorPickerOpen = false;
                colorPicker = null;
                return;
            }
            Nelianoptions.save();
            Minecraft.getMinecraft().displayGuiScreen(parent);
            return;
        }
        super.keyTyped(typedChar, keyCode);
    }

    @Override
    public boolean doesGuiPauseGame() {
        return false;
    }

    @Override
    public void onGuiClosed() {
        if (!colorPickerOpen) {
            Nelianoptions.save();
        }
        super.onGuiClosed();
    }
}
