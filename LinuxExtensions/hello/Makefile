# Настройки компилятора
CC = gcc
CFLAGS = -fPIC -Wall -O2
LDFLAGS = -shared -Wl,--version-script=exports.map

# Имена файлов
TARGET = libhello.so
SRC = hello.c
OBJ = $(SRC:.c=.o)

# Команда для проверки экспортов (наш bash-скрипт в одну строку)
CHECK_EXPORTS = nm -DC --defined-only $(TARGET) | awk '$$2 ~ /[TW]/ { print "  [EXPORTED] " $$3 }'

all: $(TARGET) check

# Сборка библиотеки
$(TARGET): $(OBJ)
	@echo "Linking $(TARGET)..."
	$(CC) $(LDFLAGS) -o $@ $^

# Компиляция объектного файла
%.o: %.c
	@echo "Compiling $<..."
	$(CC) $(CFLAGS) -c $< -o $@

# Финальный аккорд: проверка результата
check: $(TARGET)
	@echo "Verifying exported functions:"
	@$(CHECK_EXPORTS)

clean:
	rm -f $(OBJ) $(TARGET)

.PHONY: all clean check
