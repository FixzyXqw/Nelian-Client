package net.minecraft.client.Nelian.GUI;
import net.minecraft.client.gui.GuiScreen;
import net.minecraft.client.renderer.GlStateManager;
import net.minecraft.client.renderer.Tessellator;
import net.minecraft.client.renderer.WorldRenderer;
import net.minecraft.client.renderer.vertex.DefaultVertexFormats;
import org.lwjgl.opengl.GL11;
import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public class GlobalMenuBackground {

    private static final GlobalMenuBackground INSTANCE = new GlobalMenuBackground();

    public static GlobalMenuBackground get() {
        return INSTANCE;
    }

    private static final int COLOR_TOP = 0x080A0F;
    private static final int COLOR_BOTTOM = 0x111522;
    private static final float GLOW_LEFT_RADIUS = 320.0F;
    private static final float GLOW_RIGHT_RADIUS = 300.0F;
    private static final float GLOW_CENTER_RADIUS = 200.0F;
    private static final int PARTICLE_COUNT = 200;
    private static final int ORB_COUNT = 25;
    private static final float PARTICLE_MIN_SIZE = 1.0F;
    private static final float PARTICLE_MAX_SIZE = 4.5F;
    private static final float ORB_MIN_RADIUS = 60.0F;
    private static final float ORB_MAX_RADIUS = 120.0F;
    private static final float FOG_ALPHA = 0.04F;
    private final Random random = new Random();
    private long animationTick;
    private float time;
    private int particleTexture;
    private int vignetteTexture;
    private final List<Particle> particles = new ArrayList<>();
    private final List<Orb> orbs = new ArrayList<>();
    private float glowLeftX, glowLeftY;
    private float glowRightX, glowRightY;
    private float glowCenterX, glowCenterY;
    private float fogOffset;
    private boolean texturesInitialized;
    private GlobalMenuBackground() {
        initParticles();
        initOrbs();
    }
    public void render(GuiScreen screen, int width, int height) {
        renderInternal(width, height);
    }
    public void RenderCustom(int width, int height) {
        renderInternal(width, height);
    }
    public long getAnimationTick() {
        return animationTick;
    }

    private void renderInternal(int width, int height) {
        if (!texturesInitialized) {
            initTextures();
            texturesInitialized = true;
        }
        update(width, height);
        GlStateManager.disableTexture2D();
        GlStateManager.enableBlend();
        GlStateManager.blendFunc(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA);
        GlStateManager.disableAlpha();
        drawGradientBackground(width, height);
        drawAmbientGlows(width, height);
        drawFog(width, height);
        GlStateManager.enableTexture2D();
        GlStateManager.bindTexture(vignetteTexture);
        drawVignette(width, height);
        GlStateManager.bindTexture(particleTexture);
        drawOrbs(width, height);
        drawParticles(width, height);
        GlStateManager.disableTexture2D();
        GlStateManager.disableBlend();
        GlStateManager.enableTexture2D();
        GlStateManager.enableAlpha();
    }

    private void update(int width, int height) {
        animationTick++;
        time = animationTick * 0.02F;
        glowLeftX = width * 0.15F + (float) Math.sin(time * 0.31F) * 40.0F;
        glowLeftY = height * 0.20F + (float) Math.cos(time * 0.27F) * 30.0F;
        glowRightX = width * 0.85F + (float) Math.sin(time * 0.23F + 1.2F) * 50.0F;
        glowRightY = height * 0.80F + (float) Math.cos(time * 0.19F + 0.7F) * 40.0F;
        glowCenterX = width * 0.50F + (float) Math.sin(time * 0.12F) * 30.0F;
        glowCenterY = height * 0.50F + (float) Math.cos(time * 0.14F + 0.5F) * 20.0F;
        fogOffset = (float) Math.sin(time * 0.05F) * 30.0F;
        float driftX = (float) Math.sin(time * 0.011F) * 0.15F;
        float driftY = (float) Math.cos(time * 0.013F + 0.3F) * 0.10F;
        for (Particle p : particles) {
            float parallaxFactor = 1.0F / p.depth;
            p.x += p.vx + driftX * parallaxFactor;
            p.y += p.vy + driftY * parallaxFactor;
            p.currentAlpha = p.baseAlpha * (0.5F + 0.5F * (float) Math.sin(time * p.twinkleSpeed + p.phase));
            if (p.x < -20.0F || p.x > width + 20.0F || p.y < -20.0F || p.y > height + 20.0F) {
                p.x = random.nextFloat() * width;
                p.y = random.nextFloat() * height;
                p.vx = (random.nextFloat() - 0.5F) * 0.25F;
                p.vy = (random.nextFloat() - 0.5F) * 0.25F;
                p.size = PARTICLE_MIN_SIZE + random.nextFloat() * (PARTICLE_MAX_SIZE - PARTICLE_MIN_SIZE);
                p.baseAlpha = 0.15F + random.nextFloat() * 0.35F;
                p.twinkleSpeed = 0.8F + random.nextFloat() * 2.5F;
                p.phase = random.nextFloat() * 6.2832F;
                p.depth = 0.5F + random.nextFloat() * 2.0F;
            }
        }

        for (Orb o : orbs) {
            o.x += o.vx;
            o.y += o.vy;
            o.x += (float) Math.sin(time * 0.05F + o.phase) * 0.08F;
            o.y += (float) Math.cos(time * 0.06F + o.phase * 1.3F) * 0.08F;

            if (o.x < -o.radius * 2 || o.x > width + o.radius * 2 ||
                    o.y < -o.radius * 2 || o.y > height + o.radius * 2) {
                o.x = random.nextFloat() * width;
                o.y = random.nextFloat() * height;
                o.radius = ORB_MIN_RADIUS + random.nextFloat() * (ORB_MAX_RADIUS - ORB_MIN_RADIUS);
                o.vx = (random.nextFloat() - 0.5F) * 0.08F;
                o.vy = (random.nextFloat() - 0.5F) * 0.08F;
                o.alpha = 0.015F + random.nextFloat() * 0.025F;
                o.phase = random.nextFloat() * 6.2832F;
            }
        }
    }

    private void drawGradientBackground(int width, int height) {
        Tessellator tessellator = Tessellator.getInstance();
        WorldRenderer wr = tessellator.getWorldRenderer();

        int topR = (COLOR_TOP >> 16) & 0xFF;
        int topG = (COLOR_TOP >> 8) & 0xFF;
        int topB = COLOR_TOP & 0xFF;

        int bottomR = (COLOR_BOTTOM >> 16) & 0xFF;
        int bottomG = (COLOR_BOTTOM >> 8) & 0xFF;
        int bottomB = COLOR_BOTTOM & 0xFF;

        wr.begin(GL11.GL_QUADS, DefaultVertexFormats.POSITION_COLOR);

        wr.pos(0, height, 0).color(bottomR, bottomG, bottomB, 255).endVertex();
        wr.pos(width, height, 0).color(bottomR, bottomG, bottomB, 255).endVertex();
        wr.pos(width, 0, 0).color(topR, topG, topB, 255).endVertex();
        wr.pos(0, 0, 0).color(topR, topG, topB, 255).endVertex();

        tessellator.draw();
    }

    private void drawAmbientGlows(int width, int height) {
        GlStateManager.disableTexture2D();
        GlStateManager.enableBlend();
        GlStateManager.blendFunc(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA);

        drawRadialGlow(glowLeftX, glowLeftY, GLOW_LEFT_RADIUS, 0.15F, 0.40F, 0.90F, 0.25F);
        drawRadialGlow(glowRightX, glowRightY, GLOW_RIGHT_RADIUS, 0.70F, 0.20F, 0.90F, 0.20F);
        drawRadialGlow(glowCenterX, glowCenterY, GLOW_CENTER_RADIUS, 1.0F, 1.0F, 1.0F, 0.08F);

        GlStateManager.enableTexture2D();
    }

    private void drawRadialGlow(float cx, float cy, float radius, float r, float g, float b, float alpha) {
        int segments = 48;

        GL11.glPushMatrix();
        GL11.glBegin(GL11.GL_TRIANGLE_FAN);

        GL11.glColor4f(r, g, b, alpha);
        GL11.glVertex2f(cx, cy);

        for (int i = 0; i <= segments; i++) {
            double angle = 2.0 * Math.PI * i / segments;
            float x = cx + (float) (radius * Math.cos(angle));
            float y = cy + (float) (radius * Math.sin(angle));
            GL11.glColor4f(r, g, b, 0.0F);
            GL11.glVertex2f(x, y);
        }

        GL11.glEnd();
        GL11.glPopMatrix();
        GlStateManager.color(1F, 1F, 1F, 1F);
    }

    private void drawFog(int width, int height) {
        float offset = fogOffset;
        Tessellator tessellator = Tessellator.getInstance();
        WorldRenderer wr = tessellator.getWorldRenderer();

        float alpha = FOG_ALPHA;
        float topAlpha = alpha * 0.3F;
        float bottomAlpha = alpha * 0.8F;

        wr.begin(GL11.GL_QUADS, DefaultVertexFormats.POSITION_COLOR);
        float y0 = offset;
        float y1 = height + offset;

        wr.pos(0, y1, 0).color(0.1F, 0.12F, 0.18F, bottomAlpha).endVertex();
        wr.pos(width, y1, 0).color(0.1F, 0.12F, 0.18F, bottomAlpha).endVertex();
        wr.pos(width, y0, 0).color(0.1F, 0.12F, 0.18F, topAlpha).endVertex();
        wr.pos(0, y0, 0).color(0.1F, 0.12F, 0.18F, topAlpha).endVertex();
        tessellator.draw();
    }

    private void drawVignette(int width, int height) {
        GlStateManager.bindTexture(vignetteTexture);
        GlStateManager.color(1.0F, 1.0F, 1.0F, 1.0F);

        GL11.glBegin(GL11.GL_QUADS);
        GL11.glTexCoord2f(0, 0);
        GL11.glVertex2f(0, 0);
        GL11.glTexCoord2f(1, 0);
        GL11.glVertex2f(width, 0);
        GL11.glTexCoord2f(1, 1);
        GL11.glVertex2f(width, height);
        GL11.glTexCoord2f(0, 1);
        GL11.glVertex2f(0, height);
        GL11.glEnd();
    }

    private void drawParticles(int width, int height) {
        GlStateManager.bindTexture(particleTexture);
        GlStateManager.enableBlend();
        GlStateManager.blendFunc(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA);

        for (Particle p : particles) {
            if (p.currentAlpha <= 0.005F) continue;

            float size = p.size;
            float x = p.x - size / 2;
            float y = p.y - size / 2;
            float alpha = p.currentAlpha;

            GlStateManager.color(1.0F, 1.0F, 1.0F, alpha);

            GL11.glBegin(GL11.GL_QUADS);
            GL11.glTexCoord2f(0, 0);
            GL11.glVertex2f(x, y);
            GL11.glTexCoord2f(1, 0);
            GL11.glVertex2f(x + size, y);
            GL11.glTexCoord2f(1, 1);
            GL11.glVertex2f(x + size, y + size);
            GL11.glTexCoord2f(0, 1);
            GL11.glVertex2f(x, y + size);
            GL11.glEnd();
        }
    }

    private void drawOrbs(int width, int height) {
        GlStateManager.bindTexture(particleTexture);
        GlStateManager.enableBlend();
        GlStateManager.blendFunc(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA);

        for (Orb o : orbs) {
            float size = o.radius * 2;
            float x = o.x - size / 2;
            float y = o.y - size / 2;
            float alpha = o.alpha;

            GlStateManager.color(1.0F, 1.0F, 1.0F, alpha);

            GL11.glBegin(GL11.GL_QUADS);
            GL11.glTexCoord2f(0, 0);
            GL11.glVertex2f(x, y);
            GL11.glTexCoord2f(1, 0);
            GL11.glVertex2f(x + size, y);
            GL11.glTexCoord2f(1, 1);
            GL11.glVertex2f(x + size, y + size);
            GL11.glTexCoord2f(0, 1);
            GL11.glVertex2f(x, y + size);
            GL11.glEnd();
        }
    }

    private void initTextures() {
        particleTexture = generateGlowTexture(32);
        vignetteTexture = generateVignetteTexture(64);
    }

    private int generateGlowTexture(int size) {
        ByteBuffer buffer = ByteBuffer.allocateDirect(size * size * 4);

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float dx = x - size / 2.0F;
                float dy = y - size / 2.0F;
                float dist = (float) Math.sqrt(dx * dx + dy * dy);
                float maxDist = size / 2.0F;
                float alpha = 0.0F;

                if (dist < maxDist) {
                    alpha = 1.0F - (dist / maxDist);
                    alpha = alpha * alpha * (3 - 2 * alpha);
                }

                int a = (int) (alpha * 255);
                buffer.put((byte) 255);
                buffer.put((byte) 255);
                buffer.put((byte) 255);
                buffer.put((byte) a);
            }
        }
        buffer.flip();

        int texId = GL11.glGenTextures();
        GL11.glBindTexture(GL11.GL_TEXTURE_2D, texId);
        GL11.glTexImage2D(GL11.GL_TEXTURE_2D, 0, GL11.GL_RGBA, size, size, 0,
                GL11.GL_RGBA, GL11.GL_UNSIGNED_BYTE, buffer);
        GL11.glTexParameteri(GL11.GL_TEXTURE_2D, GL11.GL_TEXTURE_MIN_FILTER, GL11.GL_LINEAR);
        GL11.glTexParameteri(GL11.GL_TEXTURE_2D, GL11.GL_TEXTURE_MAG_FILTER, GL11.GL_LINEAR);
        GL11.glTexParameteri(GL11.GL_TEXTURE_2D, GL11.GL_TEXTURE_WRAP_S, GL11.GL_CLAMP);
        GL11.glTexParameteri(GL11.GL_TEXTURE_2D, GL11.GL_TEXTURE_WRAP_T, GL11.GL_CLAMP);

        return texId;
    }

    private int generateVignetteTexture(int size) {
        ByteBuffer buffer = ByteBuffer.allocateDirect(size * size * 4);

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float dx = x - size / 2.0F;
                float dy = y - size / 2.0F;
                float dist = (float) Math.sqrt(dx * dx + dy * dy);
                float maxDist = size / 2.0F;
                float t = Math.min(dist / maxDist, 1.0F);
                float alpha = t * t * (3 - 2 * t);
                alpha *= 0.7F;

                int a = (int) (alpha * 255);
                buffer.put((byte) 0);
                buffer.put((byte) 0);
                buffer.put((byte) 0);
                buffer.put((byte) a);
            }
        }
        buffer.flip();

        int texId = GL11.glGenTextures();
        GL11.glBindTexture(GL11.GL_TEXTURE_2D, texId);
        GL11.glTexImage2D(GL11.GL_TEXTURE_2D, 0, GL11.GL_RGBA, size, size, 0,
                GL11.GL_RGBA, GL11.GL_UNSIGNED_BYTE, buffer);
        GL11.glTexParameteri(GL11.GL_TEXTURE_2D, GL11.GL_TEXTURE_MIN_FILTER, GL11.GL_LINEAR);
        GL11.glTexParameteri(GL11.GL_TEXTURE_2D, GL11.GL_TEXTURE_MAG_FILTER, GL11.GL_LINEAR);
        GL11.glTexParameteri(GL11.GL_TEXTURE_2D, GL11.GL_TEXTURE_WRAP_S, GL11.GL_CLAMP);
        GL11.glTexParameteri(GL11.GL_TEXTURE_2D, GL11.GL_TEXTURE_WRAP_T, GL11.GL_CLAMP);

        return texId;
    }

    private void initParticles() {
        for (int i = 0; i < PARTICLE_COUNT; i++) {
            Particle p = new Particle();
            p.x = random.nextFloat() * 1920;
            p.y = random.nextFloat() * 1080;
            p.vx = (random.nextFloat() - 0.5F) * 0.25F;
            p.vy = (random.nextFloat() - 0.5F) * 0.25F;
            p.size = PARTICLE_MIN_SIZE + random.nextFloat() * (PARTICLE_MAX_SIZE - PARTICLE_MIN_SIZE);
            p.baseAlpha = 0.15F + random.nextFloat() * 0.35F;
            p.twinkleSpeed = 0.8F + random.nextFloat() * 2.5F;
            p.phase = random.nextFloat() * 6.2832F;
            p.depth = 0.5F + random.nextFloat() * 2.0F;
            p.currentAlpha = p.baseAlpha;
            particles.add(p);
        }
    }

    private void initOrbs() {
        for (int i = 0; i < ORB_COUNT; i++) {
            Orb o = new Orb();
            o.x = random.nextFloat() * 1920;
            o.y = random.nextFloat() * 1080;
            o.radius = ORB_MIN_RADIUS + random.nextFloat() * (ORB_MAX_RADIUS - ORB_MIN_RADIUS);
            o.vx = (random.nextFloat() - 0.5F) * 0.08F;
            o.vy = (random.nextFloat() - 0.5F) * 0.08F;
            o.alpha = 0.015F + random.nextFloat() * 0.025F;
            o.phase = random.nextFloat() * 6.2832F;
            orbs.add(o);
        }
    }

    private static class Particle {
        float x, y;
        float vx, vy;
        float size;
        float baseAlpha;
        float currentAlpha;
        float twinkleSpeed;
        float phase;
        float depth;
    }

    private static class Orb {
        float x, y;
        float radius;
        float vx, vy;
        float alpha;
        float phase;
    }
}
