VAULT_FILE=ansible/group_vars/all/vault.yml
# Файл пароля хранится в домашней папке WSL: на /mnt/* все файлы исполняемые, и Ansible пытается запустить его как скрипт
VAULT_PASS_FILE=$(HOME)/.ansible/metarankings-vault-pass
VAULT_PASS=--vault-password-file $(VAULT_PASS_FILE)

# ansible.cfg на /mnt/* игнорируется (каталог world-writable), поэтому настройки задаются через окружение
export ANSIBLE_DEPRECATION_WARNINGS=False
export ANSIBLE_PYTHON_INTERPRETER=auto_silent

# SSH-ключ, которым GitHub Actions заходит на сервер под пользователем deploy
CI_KEY=$(HOME)/.ssh/metarankings-ci

.PHONY: install vault bootstrap ci-key deploy deploy-db deploy-redis deploy-api deploy-proxy

install:
	ansible-galaxy install -r ansible/requirements.yml

vault:
	@mkdir -p $(dir $(VAULT_PASS_FILE))
	@test -f $(VAULT_PASS_FILE) || (openssl rand -hex 12 > $(VAULT_PASS_FILE) && chmod 600 $(VAULT_PASS_FILE))
	@echo 'db_password: "$(db_pass)"' > $(VAULT_FILE)
	@echo 'redis_password: "$(redis_pass)"' >> $(VAULT_FILE)
	ansible-vault encrypt $(VAULT_FILE) $(VAULT_PASS)

bootstrap:
	ANSIBLE_HOST_KEY_CHECKING=False ansible-playbook ansible/bootstrap.yml -i ansible/inventory.ini $(VAULT_PASS) -e "ansible_user=root" -k

# Генерирует ключ для CI и добавляет его публичную часть на сервер.
# Затем в GitHub (Settings → Secrets and variables → Actions) нужно задать секреты:
#   SSH_PRIVATE_KEY        — содержимое $(CI_KEY)
#   SSH_KNOWN_HOSTS        — вывод `ssh-keyscan <ip сервера>`
#   ANSIBLE_VAULT_PASSWORD — содержимое $(VAULT_PASS_FILE)
ci-key:
	@test -f $(CI_KEY) || ssh-keygen -t ed25519 -N "" -C metarankings-ci -f $(CI_KEY)
	ansible all -i ansible/inventory.ini -b -m ansible.posix.authorized_key -a "user=deploy state=present key='$$(cat $(CI_KEY).pub)'"

# Полный деплой всего (bootstrap/db/redis/api/proxy в одном запуске)
deploy:
	ansible-playbook ansible/deploy.yml $(VAULT_PASS) -i ansible/inventory.ini

# Деплой только части, ограничиваемся группами хостов
deploy-db:
	ansible-playbook ansible/deploy.yml $(VAULT_PASS) -i ansible/inventory.ini -l db

deploy-redis:
	ansible-playbook ansible/deploy.yml $(VAULT_PASS) -i ansible/inventory.ini -l redis

deploy-api:
	ansible-playbook ansible/deploy.yml $(VAULT_PASS) -i ansible/inventory.ini -l api

deploy-proxy:
	ansible-playbook ansible/deploy.yml $(VAULT_PASS) -i ansible/inventory.ini -l proxy