# 🧱 Dockbox

**Dockbox** is a declarative Docker provisioning tool for Linux systems. Just describe your containers, networks, volumes, and images in simple JSON files, and Dockbox handles the rest — apply, destroy, or dry-run your entire Docker stack with a single command.

---

## 🚀 Features

- ✅ Declarative provisioning via JSON
- 🔄 `--apply` and `--destroy` modes
- 🧪 `--dry-run` for safe previews
- 🧊 Supports:
  - Docker containers
  - Named & host-mounted volumes
  - Docker networks
  - Healthchecks & restart policies
  - Docker image pre-pull
- 📂 Reads all JSON files from `/etc/dockbox/` by default
- 📦 Compiled single-binary CLI

---

## 📦 Example Config (`/etc/dockbox/mysql.json`)

```json
{
  "images": [
    { "name": "mysql:latest", "pullIfMissing": true }
  ],
  "networks": [
    { "name": "ztacs", "driver": "bridge" }
  ],
  "volumes": [
    { "name": "mysql_data", "type": "named" }
  ],
  "containers": [
    {
      "name": "mysql",
      "image": "mysql:latest",
      "network": "ztacs",
      "restartPolicy": "unless-stopped",
      "ports": ["3306:3306"],
      "environment": {
        "MYSQL_ROOT_PASSWORD": "Redacted",
        "MYSQL_DATABASE": "Redatced",
        "MYSQL_USER": "Redacted",
        "MYSQL_PASSWORD": "Redacgted"
      },
      "volumes": [
        { "source": "mysql_data", "target": "/var/lib/mysql", "readOnly": false }
      ],
      "healthCheck": {
        "test": ["CMD", "mysqladmin", "ping", "-h", "localhost"],
        "interval": "10s",
        "timeout": "5s",
        "retries": 5
      }
    }
  ]
}
