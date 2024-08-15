import Image from "next/image";
import styles from "./page.module.css";
import { Button } from "@gravity-ui/uikit";


export default function Home() { 
  return (
    <main > 
      <h1>TheBoard</h1>
      <Button view="action" size="l">Button</Button>
    </main>
  );
}
