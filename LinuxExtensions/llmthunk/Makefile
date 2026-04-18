# Настройки компилятора
CC = gcc
CFLAGS = -fPIC -Wall -Os -flto -ffunction-sections -fdata-sections
# LDFLAGS обычно пустой, если библиотеки стандартные
LDFLAGS = -s
# Библиотеки указываем здесь
LDLIBS = -lm -luuid -flto -Wl,--gc-sections

# Имена файлов
EXE_TARGET = llmexe
SO_TARGET  = libllm.so
SRC = main.c library.c
HEADERS = llm.h
OBJ = $(SRC:.c=.o)

all: $(EXE_TARGET) $(SO_TARGET) check

# Сборка исполняемого файла
$(EXE_TARGET): $(OBJ) $(HEADERS)
	@echo "Linking executable $(EXE_TARGET)..."
	$(CC) $(LDFLAGS) -o $@ $^ $(LDLIBS)

# Сборка динамической библиотеки
$(SO_TARGET): $(OBJ) $(HEADERS)
	@echo "Linking shared library $(SO_TARGET)..."
	$(CC) -shared $(LDFLAGS) -o $@ $^ $(LDLIBS)

# Компиляция объектного файла
%.o: %.c
	@echo "Compiling $<..."
	$(CC) $(CFLAGS) -c $< -o $@

# Финальный аккорд: проверка результата
check: $(SO_TARGET)
	@echo "Verifying exported functions:"
	@nm -DC --defined-only $(SO_TARGET) | awk '$$2 ~ /[TW]/ { print "  [EXPORTED] " $$3 }'

clean:
	rm -f $(OBJ) $(EXE_TARGET) $(SO_TARGET)

.PHONY: all clean check
