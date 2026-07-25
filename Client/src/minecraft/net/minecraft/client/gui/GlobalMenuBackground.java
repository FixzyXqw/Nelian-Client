package net.minecraft.client.gui;

import net.minecraft.client.renderer.GlStateManager;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public class GlobalMenuBackground {

    private static final GlobalMenuBackground INSTANCE =
            new GlobalMenuBackground();

    public static GlobalMenuBackground get() {
        return INSTANCE;
    }

    private final Random random = new Random();

    private final List<SnowParticle> snowParticles =
            new ArrayList<SnowParticle>();

    private long animationTick;

    private GlobalMenuBackground() {

        for (int i = 0; i < 140; i++) {

            snowParticles.add(
                    new SnowParticle(
                            random.nextFloat() * 1920F,
                            random.nextFloat() * 1080F,
                            0.2F + random.nextFloat() * 1.2F,
                            1 + random.nextInt(2)
                    )
            );
        }
    }

    public void render(GuiScreen screen, int width, int height) {

        update(width, height);

        screen.drawGradientRect(
                0,
                0,
                width,
                height,
                0xFF090909,
                0xFF151515
        );

        screen.drawGradientRect(
                0,
                0,
                width,
                height,
                0x22000000,
                0x66000000
        );

        GlStateManager.disableTexture2D();
        GlStateManager.enableBlend();

        for (SnowParticle particle : snowParticles) {

            int alpha =
                    40 +
                    (int)((particle.y / (float)height) * 70);

            Gui.drawRect(
                    (int)particle.x,
                    (int)particle.y,
                    (int)particle.x + particle.size,
                    (int)particle.y + particle.size,
                    (alpha << 24) | 0xFFFFFF
            );
        }


    

        GlStateManager.disableTexture2D();
        GlStateManager.enableBlend();

        for (SnowParticle particle : snowParticles) {

            int alpha =
                    40 +
                    (int)((particle.y / (float)height) * 70);

            Gui.drawRect(
                    (int)particle.x,
                    (int)particle.y,
                    (int)particle.x + particle.size,
                    (int)particle.y + particle.size,
                    (alpha << 24) | 0xFFFFFF
            );
        }

        GlStateManager.enableTexture2D();
        GlStateManager.disableBlend();
    }

    private void update(int width, int height) {

        animationTick++;

        for (SnowParticle particle : snowParticles) {

            particle.y += particle.speed;

            if (particle.y > height) {

                particle.y = -5;

                particle.x =
                        random.nextFloat() * width;
            }
        }
    }

    public long getAnimationTick() {
        return animationTick;
    }

    private static class SnowParticle {

        float x;
        float y;
        float speed;
        int size;

        public SnowParticle(float x, float y, float speed, int size) {
            this.x = x;
            this.y = y;
            this.speed = speed;
            this.size = size;
        }
    }
}
