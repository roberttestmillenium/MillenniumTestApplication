import pandas as pd
import json
import re

def natural_sort_key(s):
    """
    Funkcja zwraca klucz do sortowania naturalnego,
    dzięki czemu "ACTION2" posortuje się przed "ACTION10".
    """
    return [int(text) if text.isdigit() else text.lower() for text in re.split(r'(\d+)', s)]

# Lista obsługiwanych typów kart i statusów
card_types = ["PREPAID", "DEBIT", "CREDIT"]
statuses = ["ORDERED", "INACTIVE", "ACTIVE", "RESTRICTED", "BLOCKED", "EXPIRED", "CLOSED"]

# Wczytanie pliku CSV (format: tab-delimited) – nazwij plik np. "card_rules_table.csv"
df = pd.read_csv("card_rules_table.csv", delimiter="\t")

# Słownik przechowujący reguły – klucz = (card_type, status, require_pin), wartość = zbiór akcji
rules = {}

for _, row in df.iterrows():
    action = row["Allowed Action"].strip()
    # Dla każdego typu karty i statusu sprawdzamy wartość zarówno w kolumnach odpowiadających typom jak i statusom.
    for ct in card_types:
        val_type = str(row[ct]).strip().upper()
        for st in statuses:
            val_status = str(row[st]).strip().upper()
            # Jeśli którakolwiek z wartości wynosi "NIE", oznacza to, że akcja nie jest dozwolona dla tej kombinacji.
            if val_type == "NIE" or val_status == "NIE":
                continue

            # Ustal wymaganie PIN-u – jeżeli w którejś z wartości pojawi się fragment "GDY PIN"
            require_pin = ("GDY PIN" in val_type) or ("GDY PIN" in val_status)
            key = (ct, st, require_pin)
            if key not in rules:
                rules[key] = set()
            rules[key].add(action)

# Konwersja zgrupowanych reguł do listy słowników
output = []
for (ct, st, req_pin), actions in rules.items():
    entry = {
        "cardTypes": [ct],
        "cardStatuses": [st],
        "actions": sorted(list(actions), key=natural_sort_key)
    }
    if req_pin:
        entry["requirePin"] = True
    output.append(entry)

# Opcjonalnie: posortuj wyniki głównej listy według cardTypes, cardStatuses, requirePin
output = sorted(output, key=lambda x: (x["cardTypes"][0], x["cardStatuses"][0], x.get("requirePin", False)))

# Zapis do pliku JSON – np. "card_rules.json"
with open("card_rules.json", "w", encoding="utf-8") as f:
    json.dump(output, f, indent=2, ensure_ascii=False)

print("Gotowe! Reguły zapisano w pliku 'card_rules.json'")
