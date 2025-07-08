# How to Connect to the Database via pgAdmin

This guide explains how to connect to the PostgreSQL database running in a Docker container using pgAdmin.

## Prerequisites

- Docker is installed and running.
- The application stack is running via `docker-compose up`.

## 1. Access pgAdmin

Open your web browser and navigate to the following URL:

[http://localhost:5050](http://localhost:5050)

## 2. Log in to pgAdmin

Use the following credentials to log in. These are defined in the `docker-compose.override.yml` file.

- **Email**: `admin@admin.com`
- **Password**: `admin`

## 3. Add a New Server Connection

After logging in, you need to add a new server to connect to the PostgreSQL database container.

1.  Right-click on **Servers** in the left-hand browser tree and select **Create** -> **Server...**.
2.  A dialog window will open.

### General Tab

- **Name**: Enter a descriptive name for your server connection, for example, `sls-db-local`.

### Connection Tab

Fill in the connection details as follows:

- **Host name/address**: `postgres`
  - *Note: We use the service name `postgres` because both pgAdmin and the database are on the same Docker network.*
- **Port**: `5432`
- **Maintenance database**: `sls_db`
- **Username**: `postgres`
- **Password**: `postgres`

## 4. Save the Connection

Click the **Save** button. The new server connection will appear under the **Servers** group in the browser tree. You can now expand it to browse and manage the `sls_db` database.
