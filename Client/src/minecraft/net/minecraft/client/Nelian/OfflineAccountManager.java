package net.minecraft.client.Nelian;

import net.minecraft.client.Minecraft;
import net.minecraft.util.Session;

import java.io.*;
import java.lang.reflect.Field;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

public class OfflineAccountManager {

    private static final File FILE = new File(
            System.getenv("APPDATA"),
            "Microsoft\\Game\\Nelian\\offline.key" //the file directory for the accounts
    );

    public static List<String> getAccounts() {
        List<String> accounts = new ArrayList<String>();

        if (!FILE.exists()) {
            createFile();
            return accounts;
        }

        try {
            BufferedReader reader = new BufferedReader(
                    new InputStreamReader(
                            new FileInputStream(FILE),
                            StandardCharsets.UTF_8
                    )
            );

            String line;

            while ((line = reader.readLine()) != null) {
                line = line.trim();

                if (!line.isEmpty() && !accounts.contains(line)) {
                    accounts.add(line);
                }
            }

            reader.close();
        } catch (Exception e) {
            e.printStackTrace();
        }

        return accounts;
    }

    public static boolean addAccount(String username) {
        username = username.trim();

        if (!isValidUsername(username)) {
            return false;
        }

        List<String> accounts = getAccounts();

        if (accounts.contains(username)) {
            return false;
        }

        accounts.add(username);
        saveAccounts(accounts);

        return true;
    }

    public static boolean isValidUsername(String username) {
        if (username == null) {
            return false;
        }

        if (username.length() < 3 || username.length() > 16) {
            return false;
        }

        for (int i = 0; i < username.length(); i++) {
            char c = username.charAt(i);

            if (!((c >= 'a' && c <= 'z') ||
                  (c >= 'A' && c <= 'Z') ||
                  (c >= '0' && c <= '9') ||
                  c == '_')) {
                return false; // doesnt save the account
            }
        }

        return true;
    }

    public static void removeAccount(String username) {
        List<String> accounts = getAccounts();

        if (accounts.remove(username)) {
            saveAccounts(accounts);
        }
    }

    private static void saveAccounts(List<String> accounts) {
        createFile();

        try {
            BufferedWriter writer = new BufferedWriter(
                    new OutputStreamWriter(
                            new FileOutputStream(FILE, false),
                            StandardCharsets.UTF_8
                    )
            );

            for (String account : accounts) {
                writer.write(account);
                writer.newLine();
            }

            writer.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private static void createFile() {
        try {
            File parent = FILE.getParentFile();

            if (!parent.exists()) {
                parent.mkdirs();
            }

            if (!FILE.exists()) {
                FILE.createNewFile();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static boolean login(String username) {
        if (!isValidUsername(username)) {
            return false;
        }

        try {
            Minecraft minecraft = Minecraft.getMinecraft();

            UUID uuid = UUID.nameUUIDFromBytes(
                    ("nelianofflineplayer" + username).getBytes(StandardCharsets.UTF_8) // could be only (username).Getbytes(...) but we lose nothing so
            );

            Session session = new Session(
                    username,
                    uuid.toString(),
                    "",
                    "legacy" //new session
            );

            Field sessionField = Minecraft.class.getDeclaredField("session");
            sessionField.setAccessible(true);
            sessionField.set(minecraft, session);

            return true;
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }

    public static File getFile() {
        return FILE; //pretty basic ain't it?
    }
}
