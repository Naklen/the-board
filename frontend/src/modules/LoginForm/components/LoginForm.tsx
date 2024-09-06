"use client";
import AuthForm from "@/components/AuthForm/AuthForm";
import { Button, Form } from "antd";
import Link from "next/link";
import { handleLogin } from "../handlers/handleLogin";
import EmailInput from "./EmailInput";
import PasswordInput from "./PasswordInput";

export function LoginForm() {
    return (
        <AuthForm onFinish={handleLogin}>
            <h1>Log in</h1>
            <EmailInput />
            <PasswordInput />
            <Form.Item>
                <Button block type="primary" htmlType="submit">
                    Log in
                </Button>
                or <Link href={"/register"}>register</Link>.
            </Form.Item>
        </AuthForm>
    );
}
