# Deployment Guide (Azure/VM)

This guide documents a minimal production deployment path for DnD Character Sheet Creator.

## 1. Build and publish

Run from repository root:

```powershell
dotnet publish DnD_Character_Sheet_Creator/DnD_Character_Sheet_Creator.Web/DnD_Character_Sheet_Creator.Web.csproj -c Release -o ./publish/web
```

Or use the helper script:

```powershell
powershell -ExecutionPolicy Bypass -File DnD_Character_Sheet_Creator/scripts/publish_release.ps1
```

## 2. Required production configuration

Set these as environment variables (or Azure App Service Configuration values):

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection=<production connection string>`
- `Authentication__Google__ClientId=<google client id>`
- `Authentication__Google__ClientSecret=<google client secret>`

Optional logging override:

- `Serilog__MinimumLevel__Default=Information`

## 3. Database migration behavior

On application startup (non-Testing environment), the app calls EF Core `Migrate()` and performs initial seed when player table is empty.

## 4. Azure App Service notes

- Runtime stack: .NET 9 (or latest compatible)
- Configure the above app settings in Azure portal
- Deploy content of `publish/web`
- Ensure outbound SQL connectivity is allowed (firewall/network rules)

## 5. VM (IIS/Kestrel + reverse proxy) notes

- Install .NET Hosting Bundle (if IIS)
- Copy `publish/web` output to server
- Configure environment variables in host environment
- Use HTTPS and reverse proxy (IIS or Nginx)

## 6. Quick smoke checks after deploy

- App starts without 5xx on `/`
- Login/register works
- Players and characters pages load
- API endpoints respond (authorized routes under authenticated session)
- Logs are written according to configured sink

---

## Windows VM + IIS + SQL Server (Detailed Steps)

### Prerequisites

- Windows Server 2019+ or Windows 10+ Pro/Enterprise
- SQL Server 2019+ (Express, Standard, or Enterprise) installed and running
- Administrator access to the VM
- Domain name or static IP (for HTTPS certificate)

### Step 1: Install .NET Hosting Bundle on VM

1. Download .NET 9 Hosting Bundle for Windows (or latest compatible):
   - Go to https://dotnet.microsoft.com/download/dotnet
   - Select version 9.0 → Windows Hosting Bundle (.exe)

2. Run the installer with Administrator privileges:
   ```
   dotnet-hosting-9.x.x-win.exe
   ```

3. Restart IIS (or reboot VM if prompted):
   ```
   iisreset /restart
   ```

### Step 2: Enable IIS and ASP.NET Core Module (if needed)

1. Open **Turn Windows features on or off** (Windows + R → `optionalfeatures`)

2. Check these boxes:
   - ☑ Internet Information Services (IIS)
     - ☑ World Wide Web Services
       - ☑ Application Development Features
         - ☑ ASP.NET 4.8+ (or .NET Framework if available)

3. Click OK and reboot if prompted.

### Step 3: Prepare SQL Server

1. Ensure SQL Server is running:
   - Open Services (`services.msc`)
   - Look for "SQL Server (SQLEXPRESS)" or your named instance
   - Start it if not running

2. Note your connection string format:
   ```
   Server=.\SQLEXPRESS;Database=DnDSheetManager;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True
   ```
   - If SQL Server is on a different machine:
   ```
   Server=<vm-ip-or-hostname>;Database=DnDSheetManager;User Id=sa;Password=<password>;TrustServerCertificate=True;MultipleActiveResultSets=True
   ```

3. (Optional) Test connection from your dev machine to verify firewall/network access.

### Step 4: Publish the Application

On your dev machine (not the VM yet):

```powershell
cd C:\path\to\DnD_Character_Sheet_Creator
powershell -ExecutionPolicy Bypass -File DnD_Character_Sheet_Creator/scripts/publish_release.ps1
```

This creates `publish/web` folder with all binaries.

### Step 5: Copy Published Output to VM

1. ZIP the `publish/web` folder:
   ```
   C:\path\to\publish\web → publish_web.zip
   ```

2. Copy to VM (via RDP file share, SCP, or USB):
   - Target location: `C:\inetpub\DnDCharacterSheet` (create this folder)

3. Extract ZIP to that folder.

### Step 6: Create IIS Application Pool

1. Open **IIS Manager** on VM:
   - Windows + R → `inetmgr`

2. In left panel, expand server → **Application Pools**

3. Right-click **Application Pools** → **Add Application Pool...**

4. Name: `DnDCharacterSheetPool`

5. .NET CLR version: **No Managed Code** (important for .NET Core)

6. Managed Pipeline Mode: **Integrated**

7. Click OK

8. Select the new pool → **Advanced Settings** (right panel):
   - **Start Mode**: AlwaysRunning
   - **Idle Time-out (minutes)**: 0 (prevent recycling)

### Step 7: Create IIS Site

1. In IIS Manager, expand server → **Sites**

2. Right-click **Sites** → **Add Website...**

3. Fill in:
   - **Site name**: DnDCharacterSheet
   - **Application pool**: DnDCharacterSheetPool
   - **Physical path**: `C:\inetpub\DnDCharacterSheet`
   - **Binding type**: https (recommended) or http
   - **IP address**: All Unassigned
   - **Port**: 443 (https) or 80 (http)
   - **Host name**: your-domain.com (or blank if IP-only)
   - **SSL certificate**: Select your cert (or self-signed for testing)

4. Click OK

### Step 8: Set Environment Variables

1. Open **Environment Variables** on VM:
   - Windows + R → `sysdm.cpl` → Advanced → Environment Variables

2. Click **New** (under System variables) and add:

   | Variable | Value |
   |----------|-------|
   | ASPNETCORE_ENVIRONMENT | Production |
   | ConnectionStrings__DefaultConnection | Server=.\SQLEXPRESS;Database=DnDSheetManager;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True |
   | Authentication__Google__ClientId | (your Google OAuth client ID) |
   | Authentication__Google__ClientSecret | (your Google OAuth secret) |

3. Click OK and reboot the VM.

### Step 9: Configure HTTPS (Optional but Recommended)

1. If using self-signed cert (testing only):
   ```
   New-SelfSignedCertificate -DnsName localhost, your-vm-hostname -CertStoreLocation "cert:\LocalMachine\My"
   ```

2. If using a real certificate (production):
   - Obtain from Let's Encrypt, DigiCert, or your CA
   - Install in `Certificates (Local Computer) → Personal → Certificates`
   - In IIS site binding, select the certificate

### Step 10: Test the Application

1. Open a browser on the VM:
   - https://localhost/ (or https://your-domain.com)

2. Check for these signs of success:
   - ✓ Home page loads without 5xx error
   - ✓ Login page works
   - ✓ Register new user works
   - ✓ After login, Players page loads
   - ✓ Create a new player (test CRUD)
   - ✓ Logs appear in `C:\inetpub\DnDCharacterSheet\Logs\` folder

3. If issues occur:
   - Check Event Viewer (Windows Logs → Application)
   - Check IIS logs: `C:\inetpub\logs\LogFiles\`
   - Verify connection string and SQL Server access

### Step 11: Configure Firewall (if accessing from outside VM)

1. Open **Windows Defender Firewall with Advanced Security**

2. **Inbound Rules** → New Rule:
   - Rule type: Port
   - Protocol: TCP
   - Specific local port: 443 (and/or 80)
   - Action: Allow
   - Name: IIS-HTTPS (or IIS-HTTP)

### Step 12: Monitor and Maintenance

1. **Restart IIS** if app stops responding:
   ```
   iisreset /restart
   ```

2. **Check logs** regularly:
   - App logs: `C:\inetpub\DnDCharacterSheet\Logs\`
   - IIS logs: `C:\inetpub\logs\LogFiles\W3SVC1\`

3. **Backup database**:
   - Scheduled SQL Server backups to a network share or blob storage

4. **Monitor app pool**:
   - In IIS Manager, right-click app pool → View App Pool Recycling Events

That's it! Your app should now be running on Windows IIS with SQL Server.

If you hit any issues during this process, let me know the specific error or step number.

