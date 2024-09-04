import { ReactNode } from "react";
import classes from "./layout.module.css";

export default function AuthLayout({
    children,
}: Readonly<{ children: React.ReactNode }>): ReactNode {
    return <div className={classes["auth-layout"]}>{children}</div>;
}
