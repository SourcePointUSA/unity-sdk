package org.json;

import java.util.LinkedHashMap;
import java.util.Map;

public final class JSONObject {
    public static final Object NULL = new Object() {
        @Override
        public String toString() {
            return "null";
        }
    };

    private final Map<String, Object> values = new LinkedHashMap<>();

    public JSONObject put(String key, Object value) {
        values.put(key, value);
        return this;
    }

    public JSONObject(Map<String, ?> values) {
        this.values.putAll(values);
    }

    public JSONObject() {
    }

    @Override
    public String toString() {
        StringBuilder json = new StringBuilder("{");
        boolean first = true;
        for (Map.Entry<String, Object> entry : values.entrySet()) {
            if (!first) {
                json.append(',');
            }
            first = false;
            json.append('"').append(entry.getKey()).append("\":");
            Object value = entry.getValue();
            if (value == null || value == NULL) {
                json.append("null");
            } else if (value instanceof String) {
                json.append('"').append(value).append('"');
            } else {
                json.append(value);
            }
        }
        return json.append('}').toString();
    }
}
