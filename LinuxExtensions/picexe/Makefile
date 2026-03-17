# Настройки компилятора
CC = gcc
CFLAGS = -fPIC -Wall -Os -flto -ffunction-sections -fdata-sections
# LDFLAGS обычно пустой, если библиотеки стандартные
LDFLAGS = -s
# Библиотеки указываем здесь
LDLIBS = -lm -flto -Wl,--gc-sections

# Имена файлов
TARGET = picexe
SRC = main.c
OBJ = $(SRC:.c=.o)

all: $(TARGET)

# Сборка программы
$(TARGET): $(OBJ)
	@echo "Linking $(TARGET)..."
	$(CC) $(LDFLAGS) -o $@ $^ $(LDLIBS)

# Компиляция объектного файла
%.o: %.c
	@echo "Compiling $<..."
	$(CC) $(CFLAGS) -c $< -o $@

clean:
	rm -f $(OBJ) $(TARGET)

.PHONY: all clean
