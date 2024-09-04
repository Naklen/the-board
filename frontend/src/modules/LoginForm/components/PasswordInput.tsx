import { Form, Input } from "antd";
import { Rule } from "antd/es/form";
import React from "react";

export default function PasswordInput() {
    const passwordRules: Rule[] = [
        {
            required: true,
            message: "Please input password",
        },
    ];
    return (
        <Form.Item
            name="password"
            label="Password"
            required
            rules={passwordRules}>
            <Input.Password type="password" placeholder="Password" />
        </Form.Item>
    );
}
