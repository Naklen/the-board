import { Form, Input } from "antd";
import { Rule } from "antd/es/form";

export default function EmailInput() {
    const emailRules: Rule[] = [
        {
            required: true,
            message: "Please input email",
        },
        {
            type: "email",
            message: "The input is not valid email",
        },
    ];
    return (
        <Form.Item
            name="email"
            label="Email"
            required
            rules={emailRules}>
            <Input type="email" placeholder="Email" />
        </Form.Item>
    )
}
