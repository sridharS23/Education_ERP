# PostgreSQL Setup Guide for Education ERP

## Local Development Setup

### Step 1: Install PostgreSQL

#### Windows

1. Download PostgreSQL from: https://www.postgresql.org/download/windows/
2. Run the installer
3. During installation:
   - Set a password for the `postgres` user (remember this!)
   - Default port: `5432`
   - Install pgAdmin 4 (GUI tool)

#### macOS

```bash
# Using Homebrew
brew install postgresql@15
brew services start postgresql@15
```

#### Linux (Ubuntu/Debian)

```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql
```

### Step 2: Verify Installation

```bash
# Check PostgreSQL version
psql --version

# Should output something like: psql (PostgreSQL) 15.x
```

### Step 3: Create Development Database

#### Option 1: Using psql (Command Line)

```bash
# Connect to PostgreSQL as postgres user
psql -U postgres

# You'll be prompted for the password you set during installation
```

Once connected, run these SQL commands:

```sql
-- Create the development database
CREATE DATABASE education_erp_dev;

-- Create a dedicated user (optional but recommended)
CREATE USER erp_dev_user WITH PASSWORD 'dev123';

-- Grant privileges
GRANT ALL PRIVILEGES ON DATABASE education_erp_dev TO erp_dev_user;

-- Connect to the database
\c education_erp_dev

-- Grant schema privileges
GRANT ALL ON SCHEMA public TO erp_dev_user;

-- Exit psql
\q
```

#### Option 2: Using pgAdmin 4 (GUI)

1. Open pgAdmin 4
2. Connect to your local PostgreSQL server
3. Right-click on "Databases" → "Create" → "Database"
4. Database name: `education_erp_dev`
5. Owner: `postgres` (or `erp_dev_user` if you created it)
6. Click "Save"

### Step 4: Update Connection String

Edit `src/EducationERP.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=education_erp_dev;Username=postgres;Password=YOUR_PASSWORD_HERE"
  }
}
```

**Replace `YOUR_PASSWORD_HERE` with your actual postgres password.**

If you created a dedicated user:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=education_erp_dev;Username=erp_dev_user;Password=dev123"
  }
}
```

### Step 5: Test Connection

```bash
# From the project root
cd src/EducationERP.API

# Test if the connection works (this will be available after we create DbContext)
dotnet ef database update --project ../EducationERP.Infrastructure
```

## Connection String Format

### Basic Format

```
Host=localhost;Port=5432;Database=database_name;Username=user;Password=pass
```

### With SSL (Production)

```
Host=prod-server.com;Port=5432;Database=education_erp_prod;Username=erp_user;Password=strong_pass;SSL Mode=Require
```

### Connection String Parameters

| Parameter           | Description               | Example                        |
| ------------------- | ------------------------- | ------------------------------ |
| `Host`              | Server address            | `localhost` or `192.168.1.100` |
| `Port`              | PostgreSQL port           | `5432` (default)               |
| `Database`          | Database name             | `education_erp_dev`            |
| `Username`          | Database user             | `postgres`                     |
| `Password`          | User password             | `your_password`                |
| `SSL Mode`          | SSL connection mode       | `Disable`, `Require`, `Prefer` |
| `Pooling`           | Enable connection pooling | `true` (default)               |
| `Minimum Pool Size` | Min connections           | `0` (default)                  |
| `Maximum Pool Size` | Max connections           | `100` (default)                |

### Example with Connection Pooling

```
Host=localhost;Port=5432;Database=education_erp_dev;Username=postgres;Password=pass;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=50
```

## Common Issues & Solutions

### Issue 1: "password authentication failed"

**Solution**: Double-check your password in the connection string

### Issue 2: "could not connect to server"

**Solution**:

- Ensure PostgreSQL service is running
- Windows: Check Services → PostgreSQL
- Linux: `sudo systemctl status postgresql`
- macOS: `brew services list`

### Issue 3: "database does not exist"

**Solution**: Create the database using the SQL commands above

### Issue 4: Port 5432 already in use

**Solution**:

- Check if another PostgreSQL instance is running
- Change the port in postgresql.conf
- Update connection string to use the new port

### Issue 5: Permission denied

**Solution**:

```sql
-- Connect as postgres user
psql -U postgres

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE education_erp_dev TO your_user;
GRANT ALL ON SCHEMA public TO your_user;
```

## PostgreSQL Management Tools

### 1. pgAdmin 4 (GUI)

- Comes with PostgreSQL installer
- Web-based interface
- Visual query builder
- Database backup/restore

### 2. psql (Command Line)

```bash
# Connect to database
psql -U postgres -d education_erp_dev

# Useful commands
\l              # List databases
\c dbname       # Connect to database
\dt             # List tables
\d tablename    # Describe table
\du             # List users
\q              # Quit
```

### 3. DBeaver (Cross-platform)

- Free universal database tool
- Download: https://dbeaver.io/

### 4. DataGrip (JetBrains)

- Paid IDE for databases
- Excellent for complex queries

## Database Backup & Restore

### Backup

```bash
# Backup entire database
pg_dump -U postgres -d education_erp_dev -F c -f backup.dump

# Backup as SQL script
pg_dump -U postgres -d education_erp_dev -f backup.sql
```

### Restore

```bash
# Restore from custom format
pg_restore -U postgres -d education_erp_dev backup.dump

# Restore from SQL script
psql -U postgres -d education_erp_dev -f backup.sql
```

## Production Database Setup

### 1. Create Production Database

```sql
-- Use a strong password!
CREATE USER erp_prod_user WITH PASSWORD 'STRONG_RANDOM_PASSWORD_HERE';

CREATE DATABASE education_erp_prod OWNER erp_prod_user;

-- Grant minimal required privileges
GRANT CONNECT ON DATABASE education_erp_prod TO erp_prod_user;
GRANT USAGE ON SCHEMA public TO erp_prod_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO erp_prod_user;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO erp_prod_user;
```

### 2. Update Production Connection String

**Option 1: appsettings.Production.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-db-server.com;Port=5432;Database=education_erp_prod;Username=erp_prod_user;Password=STRONG_PASSWORD;SSL Mode=Require"
  }
}
```

**Option 2: Environment Variables (Recommended)**

```bash
# Linux/macOS
export ConnectionStrings__DefaultConnection="Host=prod-server;..."

# Windows PowerShell
$env:ConnectionStrings__DefaultConnection="Host=prod-server;..."

# Docker
docker run -e ConnectionStrings__DefaultConnection="Host=..." your-image
```

### 3. Security Best Practices

✅ **DO:**

- Use strong, random passwords
- Enable SSL/TLS connections
- Use dedicated database users (not `postgres`)
- Grant minimal required privileges
- Regular backups
- Monitor database logs
- Use connection pooling
- Keep PostgreSQL updated

❌ **DON'T:**

- Use default passwords
- Expose PostgreSQL port to internet
- Use `postgres` superuser in application
- Store passwords in source control
- Grant unnecessary privileges

## Environment-Specific Configurations

### Development

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=education_erp_dev;Username=postgres;Password=dev123"
  }
}
```

### QA/Testing

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=qa-db-server;Port=5432;Database=education_erp_qa;Username=erp_qa_user;Password=qa_password;SSL Mode=Prefer"
  }
}
```

### Staging

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=staging-db-server;Port=5432;Database=education_erp_staging;Username=erp_staging_user;Password=staging_password;SSL Mode=Require"
  }
}
```

### Production

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-db-server;Port=5432;Database=education_erp_prod;Username=erp_prod_user;Password=${DB_PASSWORD};SSL Mode=Require;Pooling=true;Maximum Pool Size=100"
  }
}
```

## Monitoring & Performance

### Check Active Connections

```sql
SELECT * FROM pg_stat_activity WHERE datname = 'education_erp_dev';
```

### Check Database Size

```sql
SELECT pg_size_pretty(pg_database_size('education_erp_dev'));
```

### Check Table Sizes

```sql
SELECT
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
```

## Next Steps

1. ✅ PostgreSQL installed and running
2. ✅ Database created
3. ✅ Connection string configured
4. ⏭️ Run Entity Framework migrations
5. ⏭️ Seed initial data
6. ⏭️ Test the application

For Entity Framework migrations, see the main README.md file.
