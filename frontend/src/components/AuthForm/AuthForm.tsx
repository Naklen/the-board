import { Form } from "antd";
import React, { ReactNode } from "react";
import classes from "./AuthForm.module.css";

type AuthFormTypes = {
    onFinish: (values: any) => Promise<void>;
    children: ReactNode;
};

export default function AuthForm({ onFinish, children }: AuthFormTypes) {
    return (
        <Form name="auth" className={classes["auth-form"]} onFinish={onFinish} layout="vertical">
            {children}
        </Form>
    );
}
