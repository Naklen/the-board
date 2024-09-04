"use client";
import AuthForm from "@/components/AuthForm/AuthForm";
import { Button, Form } from "antd";
import Link from "next/link";
import React from "react";
import EmailInput from "./EmailInput";
import PasswordInput from "./PasswordInput";
export function LoginForm() {
    const onFinish = async (values: any) => {
        console.log("Received values of form: ", values);
    };

    return (
        <AuthForm onFinish={onFinish}>
            <h1>Log in</h1>
            <EmailInput/>
            <PasswordInput/>
            <Form.Item>
                <Button block type="primary" htmlType="submit">
                    Log in
                </Button>
                or <Link href={"/register"}>register</Link>.
            </Form.Item>
        </AuthForm>
    );
}
