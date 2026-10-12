import struct



wasm_path = r"D:\Projects for PC\Unity\My project\WebGLBuild\Build\WebGLBuild.wasm"



with open(wasm_path, "rb") as f:

    magic = f.read(4)

    version = f.read(4)

    sections = []

    while True:

        sec_id_bytes = f.read(1)

        if not sec_id_bytes:

            break

        sec_id = sec_id_bytes[0]



        size = 0

        shift = 0

        while True:

            b = f.read(1)[0]

            size |= (b & 0x7f) << shift

            if not (b & 0x80):

                break

            shift += 7

        pos = f.tell()

        name = ""

        if sec_id == 0:



            nlen = 0

            nshift = 0

            while True:

                b = f.read(1)[0]

                nlen |= (b & 0x7f) << nshift

                if not (b & 0x80):

                    break

                nshift += 7

            name = f.read(nlen).decode("utf-8", errors="ignore")

            f.seek(pos)

        sections.append((sec_id, name, size, pos))

        f.seek(pos + size)



for s in sections:

    print(f"Sec {s[0]}: {s[1]:<20} size={s[2]} at offset=0x{s[3]:x}")

