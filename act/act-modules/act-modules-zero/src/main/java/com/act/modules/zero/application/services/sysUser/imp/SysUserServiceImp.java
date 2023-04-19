package com.act.modules.zero.application.services.sysUser.imp;

import com.act.core.application.CurdAppService;
import com.act.core.utils.AjaxResponse;
import com.act.core.utils.JWTUtils;
import com.act.modules.zero.application.services.sysUser.SysUserService;
import com.act.modules.zero.application.services.sysUser.dto.LoginDTO;
import com.act.modules.zero.application.services.sysUser.dto.SysUserDTO;
import com.act.modules.zero.domain.SysUser;
import com.act.modules.zero.mapper.SysUserMapper;
import lombok.var;
import org.springframework.stereotype.Service;

import javax.annotation.Resource;
import java.util.Hashtable;

@Service
public class SysUserServiceImp extends CurdAppService<SysUser, SysUserDTO> implements SysUserService {

    @Resource
    private SysUserMapper sysUserMapper;

    public SysUserServiceImp() {

    }

    public AjaxResponse<Object> Login(LoginDTO request) throws InstantiationException, IllegalAccessException {

        var sysUserDTO = new SysUserDTO();

        sysUserDTO.setUserName("alangur");
        sysUserDTO.setIcon("hello");
        CreateOrEdit(sysUserDTO);

        var map = new Hashtable<String, Object>();
        map.put("userId", 1);
        var token = JWTUtils.getToken(map);
        return new AjaxResponse<Object>(token);
    }

}