package org.json;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

public final class JSONArray {
    private final List<Object> values = new ArrayList<>();

    public JSONArray() {
    }

    public JSONArray(Collection<?> values) {
        this.values.addAll(values);
    }

    public JSONArray put(Object value) {
        values.add(value);
        return this;
    }

    @Override
    public String toString() {
        return values.toString();
    }
}
