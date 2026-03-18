# Используем Debian 10 как среду сборки
FROM debian:10

# Переключаем репозитории на архивные, так как Buster больше не поддерживается
RUN sed -i 's/deb.debian.org/archive.debian.org/g' /etc/apt/sources.list && \
    sed -i 's|security.debian.org/debian-security|archive.debian.org/debian-security|g' /etc/apt/sources.list && \
    sed -i '/buster-updates/d' /etc/apt/sources.list

# Устанавливаем необходимые инструменты (gcc, make и т.д.)
RUN apt-get update && apt-get install -y \
    build-essential \
    cmake \
    # Добавьте здесь другие зависимости вашего проекта
    && rm -rf /var/lib/apt/lists/*

# Копируем исходники в контейнер
WORKDIR /app
COPY . .

# Команда для сборки (замените на свою)
RUN make

# После сборки бинарник можно будет забрать из контейнера
