package com.act.core.utils;

import lombok.Data;

@Data
public class FriendlyException extends Exception {
    private int code;
    private String msg;

    public FriendlyException(String message) {
        msg = message;
        code = -1;
    }
}