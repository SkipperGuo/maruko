package com.act.modules.zero.application.role.dto;

import lombok.Data;

import java.util.ArrayList;

@Data
public class SetRolePermissionRequest {
    private Long roleId;
    private ArrayList<String> menuIds = new ArrayList<>();
}